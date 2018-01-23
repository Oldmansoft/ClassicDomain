using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Core
{
    /// <summary>
    /// 实体集
    /// </summary>
    /// <typeparam name="TDomain"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    internal class SafeModeDbSet<TDomain, TKey> : DbSet<TDomain, TKey>
    {
        public IdentityMap<TDomain> IdentityMap { get; private set; }

        /// <summary>
        /// 创建实体集
        /// </summary>
        /// <param name="database"></param>
        /// <param name="keyExpression"></param>
        public SafeModeDbSet(MongoDatabase database, System.Linq.Expressions.Expression<Func<TDomain, TKey>> keyExpression)
            : base(database, keyExpression)
        {
            IdentityMap = new IdentityMap<TDomain>();
            IdentityMap.SetKey(keyExpression.Compile());
        }

        /// <summary>
        /// 注册添加
        /// </summary>
        /// <param name="domain"></param>
        public override void RegisterAdd(TDomain domain)
        {
            TrySetDomainKey(domain);
            domain = domain.MapTo<TDomain>();
            Commands.Enqueue(new Commands.SafeModeAddCommand<TDomain>(GetCollection(), domain, IdentityMap));
        }

        /// <summary>
        /// 注册替换
        /// </summary>
        /// <param name="domain"></param>
        public override void RegisterReplace(TDomain domain)
        {
            domain = domain.MapTo<TDomain>();
            Commands.Enqueue(new Commands.SafeModeReplaceCommand<TDomain, TKey>(KeyExpression, KeyExpressionCompile, GetCollection(), domain, IdentityMap));
        }

        /// <summary>
        /// 注册移除
        /// </summary>
        /// <param name="domain"></param>
        public override void RegisterRemove(TDomain domain)
        {
            domain = domain.MapTo<TDomain>();
            Commands.Enqueue(new Commands.SafeModeRemoveCommand<TDomain, TKey>(KeyExpression, GetCollection(), KeyExpressionCompile(domain), IdentityMap));
        }
    }
}
