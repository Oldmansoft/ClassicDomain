using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oldmansoft.ClassicDomain.Util;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Core
{
    /// <summary>
    /// 快速模式实体上下文
    /// </summary>
    public abstract class FastModeContext : Context
    {
        /// <summary>
        /// 配置
        /// </summary>
        private static Config Server { get; set; }

        static FastModeContext()
        {
            Server = new FastModeConfig();
        }
        
        /// <summary>
        /// 创建实体集
        /// </summary>
        /// <typeparam name="TDomain"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keyExpression"></param>
        /// <returns></returns>
        internal override IDbSet<TDomain, TKey> CreateDbSet<TDomain, TKey>(System.Linq.Expressions.Expression<Func<TDomain, TKey>> keyExpression)
        {
            return new FastModeDbSet<TDomain, TKey>(Server.Get(ConnectionName).GetDatabase(), keyExpression);
        }
        
        /// <summary>
        /// 获取 commit 的主机
        /// </summary>
        /// <returns></returns>
        public override string GetHost()
        {
            return Server.Get(ConnectionName).GetHost();
        }
    }
}
