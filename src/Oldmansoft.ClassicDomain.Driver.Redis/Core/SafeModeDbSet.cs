using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Core
{
    internal class SafeModeDbSet<TDomain, TKey> : DbSet<TDomain, TKey>, IMergeKey<TKey> where TDomain : class, new()
    {
        private IdentityMap<TDomain> IdentityMap { get; set; }
        
        /// <summary>
        /// 创建实体集
        /// </summary>
        /// <param name="config"></param>
        /// <param name="db"></param>
        /// <param name="commands"></param>
        /// <param name="keyExpression"></param>
        public SafeModeDbSet(ConfigItem config, IDatabase db, ConcurrentQueue<ICommand> commands, System.Linq.Expressions.Expression<Func<TDomain, TKey>> keyExpression)
            : base(config, db, commands, keyExpression)
        {
            IdentityMap = new IdentityMap<TDomain>();
            IdentityMap.SetKey(KeyExpressionCompile);
        }

        /// <summary>
        /// 注册添加
        /// </summary>
        /// <param name="domain"></param>
        public override void RegisterAdd(TDomain domain)
        {
            TrySetDomainKey(domain);
            domain = domain.MapTo<TDomain>();
            Commands.Enqueue(new Commands.SafeModeAddCommand<TDomain, TKey>(Db, MergeKey, this, IdentityMap, KeyExpressionCompile(domain), domain));
        }

        /// <summary>
        /// 注册替换
        /// </summary>
        /// <param name="domain"></param>
        public override void RegisterReplace(TDomain domain)
        {
            domain = domain.MapTo<TDomain>();
            Commands.Enqueue(new Commands.SafeModeReplaceCommand<TDomain, TKey>(Db, MergeKey, this, IdentityMap, KeyExpressionCompile(domain), domain));
        }

        /// <summary>
        /// 注册移除
        /// </summary>
        /// <param name="domain"></param>
        public override void RegisterRemove(TDomain domain)
        {
            Commands.Enqueue(new Commands.SafeModeRemoveCommand<TDomain, TKey>(Db, MergeKey, this, IdentityMap, KeyExpressionCompile(domain)));
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
            var reflection = Library.ContextGetHelper.GetReflection(typeof(TDomain));
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
            var result = Library.ContextGetHelper.GetContext<TDomain>(FromRedis(Db, id));
            IdentityMap.Set(result);
            return result;
        }
    }
}
