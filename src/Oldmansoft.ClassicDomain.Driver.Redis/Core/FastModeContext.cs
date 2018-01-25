using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
using System.Collections.Concurrent;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Core
{
    /// <summary>
    /// 快速模式上下文
    /// </summary>
    public abstract class FastModeContext : Context
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
        internal override IDbSet CreateDbSet<TDomain, TKey>(ConfigItem config, IDatabase db, ConcurrentQueue<ICommand> commands, System.Linq.Expressions.Expression<Func<TDomain, TKey>> keyExpression)
        {
            return new FastModeDbSet<TDomain, TKey>(config, db, commands, keyExpression);
        }
    }
}
