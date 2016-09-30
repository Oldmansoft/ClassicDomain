using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

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
        /// <param name="keyExpression"></param>
        /// <returns></returns>
        internal override IDbSet CreateDbSet<TDomain, TKey>(ConfigItem config, IDatabase db, Func<TDomain, TKey> keyExpression)
        {
            return new SafeModeDbSet<TDomain, TKey>(config, db, keyExpression);
        }
    }
}
