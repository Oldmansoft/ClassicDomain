using StackExchange.Redis;
using System;
using System.Collections.Concurrent;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Core
{
    /// <summary>
    /// 安全模式上下文
    /// </summary>
    public abstract class SafeModeContext : Context
    {
        /// <summary>
        /// 创建实体集
        /// </summary>
        /// <typeparam name="TDomain"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="config"></param>
        /// <param name="db"></param>
        /// <param name="commands"></param>
        /// <param name="keyExpression"></param>
        /// <returns></returns>
        internal override IDbSet CreateDbSet<TDomain, TKey>(Config config, IDatabase db, ConcurrentQueue<ICommand> commands, System.Linq.Expressions.Expression<Func<TDomain, TKey>> keyExpression)
        {
            return new SafeModeDbSet<TDomain, TKey>(config, db, commands, keyExpression);
        }
    }
}
