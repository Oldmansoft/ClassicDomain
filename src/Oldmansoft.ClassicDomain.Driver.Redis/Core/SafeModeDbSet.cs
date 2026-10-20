using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Core
{
    internal class SafeModeDbSet<TDomain, TKey> : DbSet<TDomain, TKey>, IMergeKey<TKey>, IClearIdentityMap where TDomain : class, new()
    {
        private IdentityMap<TDomain> IdentityMap { get; set; }

        /// <summary>
        /// 创建实体集
        /// </summary>
        /// <param name="config"></param>
        /// <param name="db"></param>
        /// <param name="commands"></param>
        /// <param name="keyExpression"></param>
        public SafeModeDbSet(Config config, IDatabase db, ConcurrentQueue<ICommand> commands, System.Linq.Expressions.Expression<Func<TDomain, TKey>> keyExpression)
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
            Commands.Enqueue(new Commands.AddCommand<TDomain, TKey>(Db, MergeKey, this, IdentityMap, KeyExpressionCompile(domain), domain));
        }

        /// <summary>
        /// 注册替换
        /// </summary>
        /// <param name="domain"></param>
        public override void RegisterReplace(TDomain domain)
        {
            domain = domain.MapTo<TDomain>();
            Commands.Enqueue(new Commands.ReplaceCommand<TDomain, TKey>(Db, MergeKey, this, IdentityMap, KeyExpressionCompile(domain), domain));
        }

        /// <summary>
        /// 注册移除
        /// </summary>
        /// <param name="domain"></param>
        public override void RegisterRemove(TDomain domain)
        {
            Commands.Enqueue(new Commands.RemoveCommand<TDomain, TKey>(Db, MergeKey, this, IdentityMap, KeyExpressionCompile(domain)));
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

        /// <summary>
        /// 清空标识映射
        /// </summary>
        public void ClearIdentityMap()
        {
            IdentityMap.Clear();
        }

        private Library.DataGetMapping FromRedis(IDatabase db, TKey key)
        {
            var reflection = Library.ContextGetHelper.GetReflection(typeof(TDomain));
            var batch = db.CreateBatch();
            var hashTask = batch.HashGetAllAsync(MergeKey(key));
            var listTasks = new Dictionary<string, Task<RedisValue[]>>();
            foreach (var name in reflection.ListNames)
            {
                listTasks.Add(name, batch.ListRangeAsync(MergeKey(key, name)));
            }
            var hashTasks = new Dictionary<string, Task<HashEntry[]>>();
            foreach (var name in reflection.HashNames)
            {
                hashTasks.Add(name, batch.HashGetAllAsync(MergeKey(key, name)));
            }
            batch.Execute();

            var byteNames = new HashSet<string>(reflection.ByteNames);
            var fields = new Dictionary<string, string>();
            var bytes = new Dictionary<string, byte[]>();
            foreach (var entry in hashTask.Result)
            {
                var name = (string)entry.Name;
                if (byteNames.Contains(name))
                {
                    if (!entry.Value.IsNull) bytes[name] = (byte[])entry.Value;
                    continue;
                }
                fields[name] = entry.Value;
            }

            var result = new Library.DataGetMapping(fields);
            foreach (var item in bytes) result.Bytes[item.Key] = item.Value;
            foreach (var item in listTasks)
            {
                if (!fields.ContainsKey(item.Key)) continue;
                result.Lists.Add(item.Key, item.Value.Result.ToStringList());
            }
            foreach (var item in hashTasks)
            {
                if (!fields.ContainsKey(item.Key)) continue;
                result.Hashs.Add(item.Key, item.Value.Result.ToStringDictionary());
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
