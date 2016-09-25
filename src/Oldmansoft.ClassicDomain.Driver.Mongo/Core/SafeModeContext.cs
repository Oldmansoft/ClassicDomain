using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Core
{
    /// <summary>
    /// 安全模式实体上下文
    /// </summary>
    public abstract class SafeModeContext : Context
    {
        /// <summary>
        /// 配置
        /// </summary>
        private static Config Server { get; set; }

        static SafeModeContext()
        {
            Server = new SafeModeConfig();
        }

        /// <summary>
        /// 配置项
        /// </summary>
        private ConfigItem Config { get; set; }
        
        /// <summary>
        /// 创建上下文
        /// </summary>
        public SafeModeContext()
        {
            Config = Server.Get(ConnectionName);
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
            var database = Config.GetDatabase() as Library.MongoDatabase;
            var result = new SafeModeDbSet<TDomain, TKey>(database, keyExpression);
            result.IdentityMap.SetKey(keyExpression.Compile());
            database.SetIdentityMap(result.IdentityMap);
            return result;
        }

        /// <summary>
        /// 获取 commit 的主机
        /// </summary>
        /// <returns></returns>
        public override string GetHost()
        {
            return Config.GetHost();
        }
    }
}
