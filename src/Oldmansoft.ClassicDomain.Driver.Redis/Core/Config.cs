using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Core
{
    /// <summary>
    /// 配置
    /// </summary>
    internal class Config
    {
        public const string AlertLowServerVersion = "接口不支持旧版 Redis 服务器，请换用新的 Redis 服务器，或在配置里指定 providerName=\"2\"。";

        private ConcurrentDictionary<string, ConfigItem> Connections { get; set; }

        /// <summary>
        /// 创建配置
        /// </summary>
        public Config()
        {
            Connections = new ConcurrentDictionary<string, ConfigItem>();
        }

        /// <summary>
        /// 获取配置项
        /// </summary>
        /// <param name="callerType"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public ConfigItem Get(Type callerType, string name)
        {
            ConfigItem result;
            if (Connections.TryGetValue(name, out result)) return result;

            var setting = Configuration.Config.GetConnectionStringSettings(callerType, name);

            var connection = ConnectionMultiplexer.Connect(setting.ConnectionString);
            result = new ConfigItem(connection, setting.ProviderName);
            if (!Connections.TryAdd(name, result))
            {
                connection.Close();
                Connections.TryGetValue(name, out result);
            }
            return result;
        }
    }
}
