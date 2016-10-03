using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oldmansoft.ClassicDomain.Util;
using StackExchange.Redis;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Core
{
    internal class SafeModeDbSet<TDomain, TKey> : DbSet<TDomain, TKey>, IMergeKey<TKey> where TDomain : class, new()
    {
        private ConcurrentDictionary<TKey, TDomain> IdentityMap { get; set; }

        private Library.ChangeList<TKey> ChangeList { get; set; }

        /// <summary>
        /// 创建实体集
        /// </summary>
        /// <param name="config"></param>
        /// <param name="db"></param>
        /// <param name="keyExpression"></param>
        public SafeModeDbSet(ConfigItem config, IDatabase db, System.Linq.Expressions.Expression<Func<TDomain, TKey>> keyExpression)
            : base(config, db, keyExpression)
        {
            IdentityMap = new ConcurrentDictionary<TKey, TDomain>();
            ChangeList = new Library.ChangeList<TKey>();
        }

        /// <summary>
        /// 注册添加
        /// </summary>
        /// <param name="domain"></param>
        public override void RegisterAdd(TDomain domain)
        {
            TrySetDomainKey(domain);
            ChangeList.Addes.Enqueue(Library.ContextSetAddtHelper.GetContext(KeyExpressionCompile(domain), typeof(TDomain), domain));
        }

        /// <summary>
        /// 注册替换
        /// </summary>
        /// <param name="domain"></param>
        public override void RegisterReplace(TDomain domain)
        {
            TDomain source;
            var key = KeyExpressionCompile(domain);
            if (!IdentityMap.TryGetValue(key, out source))
            {
                throw new ArgumentException("修改的实例必须经过加载", "domain");
            }
            ChangeList.Replaces.Enqueue(Library.ContextSetReplaceHelper.GetContext(key, typeof(TDomain), source, domain));
        }

        /// <summary>
        /// 注册移除
        /// </summary>
        /// <param name="domain"></param>
        public override void RegisterRemove(TDomain domain)
        {
            ChangeList.Removes.Enqueue(Library.ContextSetRemoveHelper.GetContext(KeyExpressionCompile(domain), typeof(TDomain)));
        }

        /// <summary>
        /// 创建键值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="subName"></param>
        /// <returns></returns>
        public string MergeKey(TKey key, string subName)
        {
            string id;
            if (key is Guid) id = ((Guid)Convert.ChangeType(key, typeof(Guid))).ToString("n");
            else id = key.ToString();

            return string.Format("{0}_{1}:{2}", DomainName, subName, id);
        }

        private Library.DataGetMapping FromRedis(IDatabase db, TKey key)
        {
            var reflection = Library.ContextSetGetHelper.GetReflection(typeof(TDomain));
            var result = new Library.DataGetMapping(db.HashGetAll(MergeKey(key)).ToStringDictionary());
            for (var i = 0; i < reflection.ListNames.Count; i++)
            {
                if (!result.Fields.ContainsKey(reflection.ListNames[i])) continue;
                result.Lists.Add(reflection.ListNames[i], db.ListRange(MergeKey(key, reflection.ListNames[i])).ToStringList());
            }
            for (var i = 0; i < reflection.HashNames.Count; i++)
            {
                if (!result.Fields.ContainsKey(reflection.HashNames[i])) continue;
                var hash = db.HashGetAll(MergeKey(key, reflection.HashNames[i])).ToStringDictionary();
                result.Hashs.Add(reflection.HashNames[i], hash);
            }
            return result;
        }

        public override TDomain Get(TKey id)
        {
            var result = Library.ContextSetGetHelper.GetContext<TDomain>(FromRedis(Db, id));
            IdentityMap.TryAdd(id, result.CopyTo(new TDomain()));
            return result;
        }

        public override int Commit()
        {
            var result = 0;
            Library.UpdatedCommand<TKey> command;
            while (ChangeList.Addes.TryDequeue(out command))
            {
                try
                {
                    if (!Db.HashSet(MergeKey(command.Key), "this", typeof(TDomain).FullName)) continue;
                }
                catch (RedisServerException ex)
                {
                    if (ex.Message == "ERR Operation against a key holding the wrong kind of value")
                    {
                        throw new ClassicDomainException("数据冲突：存在着相同记录的不同类型数据，可能是之前使用过快速模式保存过。");
                    }
                    else
                    {
                        throw;
                    }
                }
                command.Execute(Db, this);
                result++;
            }
            while (ChangeList.Replaces.TryDequeue(out command))
            {
                if (string.IsNullOrEmpty(Db.HashGet(MergeKey(command.Key), "this"))) continue;
                command.Execute(Db, this);
                result++;
            }
            while (ChangeList.Removes.TryDequeue(out command))
            {
                command.Execute(Db, this);
                if (!Db.KeyDelete(MergeKey(command.Key))) continue;
                TDomain temp;
                IdentityMap.TryRemove(command.Key, out temp);
                result++;
            }
            return result + base.Commit();
        }
    }
}
