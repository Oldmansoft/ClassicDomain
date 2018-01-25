using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using System.Collections.Concurrent;

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

        private ConfigItem GetConfig()
        {
            if (Config == null) Config = Server.Get(ConnectionName);
            return Config;
        }
        
        /// <summary>
        /// 创建实体集
        /// </summary>
        /// <typeparam name="TDomain"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="commands"></param>
        /// <param name="keyExpression"></param>
        /// <returns></returns>
        internal override IDbSet<TDomain, TKey> CreateDbSet<TDomain, TKey>(ConcurrentQueue<ICommand> commands, System.Linq.Expressions.Expression<Func<TDomain, TKey>> keyExpression)
        {
            var database = GetConfig().GetDatabase() as Library.MongoDatabase;
            var result = new SafeModeDbSet<TDomain, TKey>(database, commands, keyExpression);
            database.SetIdentityMap(result.IdentityMap);
            return result;
        }
    }
}