using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
using System.Collections.Concurrent;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Core
{
    internal class FastModeDbSet<TDomain, TKey> : DbSet<TDomain, TKey>
    {
        /// <summary>
        /// 创建实体集
        /// </summary>
        /// <param name="config"></param>
        /// <param name="db"></param>
        /// <param name="commands"></param>
        /// <param name="keyExpression"></param>
        public FastModeDbSet(ConfigItem config, IDatabase db, ConcurrentQueue<ICommand> commands, System.Linq.Expressions.Expression<Func<TDomain, TKey>> keyExpression)
            : base(config, db, commands, keyExpression)
        {
        }

        /// <summary>
        /// 注册添加
        /// </summary>
        /// <param name="domain"></param>
        public override void RegisterAdd(TDomain domain)
        {
            TrySetDomainKey(domain);
            domain = domain.MapTo<TDomain>();
            Commands.Enqueue(new Commands.FastModeAddCommand<TDomain>(Db, GetKey, domain));
        }

        /// <summary>
        /// 注册替换
        /// </summary>
        /// <param name="domain"></param>
        public override void RegisterReplace(TDomain domain)
        {
            domain = domain.MapTo<TDomain>();
            Commands.Enqueue(new Commands.FastModeReplaceCommand<TDomain>(Db, GetKey, Config, domain));
        }

        /// <summary>
        /// 注册移除
        /// </summary>
        /// <param name="domain"></param>
        public override void RegisterRemove(TDomain domain)
        {
            Commands.Enqueue(new Commands.FastModeRemoveCommand<TDomain>(Db, GetKey(domain)));
        }
        
        public override TDomain Get(TKey id)
        {
            var content = Db.StringGet(MergeKey(id));
            return Library.Serializer.Deserialize<TDomain>(content);
        }
    }
}
