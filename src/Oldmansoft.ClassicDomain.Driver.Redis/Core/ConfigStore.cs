using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Core
{
    /// <summary>
    /// 配置库
    /// </summary>
    internal class ConfigStore
    {
        public const string AlertLowServerVersion = "接口不支持旧版 Redis 服务器，请换用新的 Redis 服务器。";

        private Dictionary<string, Config> Connections { get; set; }

        /// <summary>
        /// 创建配置
        /// </summary>
        public ConfigStore()
        {
            Connections = new Dictionary<string, Config>();
        }

        /// <summary>
        /// 获取配置项
        /// </summary>
        /// <param name="callerType"></param>
        /// <param name="isLowServerVersion"></param>
        /// <returns></returns>
        public Config Get(Type callerType, bool isLowServerVersion)
        {
            var name = callerType.FullName;
            if (Connections.TryGetValue(name, out Config result)) return result;
            lock (Connections)
            {
                if (Connections.TryGetValue(name, out result)) return result;

                var connectionString = UnitOfWorkManagedContext.GetConnectionString(callerType);
                var connection = ConnectionMultiplexer.Connect(connectionString);
                result = new Config(connection, isLowServerVersion);
                Connections.Add(name, result);
                return result;
            }
        }
    }
}
