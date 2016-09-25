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
        /// <param name="name"></param>
        /// <returns></returns>
        public ConfigItem Get(string name)
        {
            ConfigItem result;
            if (Connections.TryGetValue(name, out result)) return result;

            var setting = Configuration.Config.GetConnectionStringSettings(name);

            var connection = ConnectionMultiplexer.Connect(setting.ConnectionString);
            result = new ConfigItem(connection, GetHost(connection), setting.ProviderName);
            if (!Connections.TryAdd(name, result))
            {
                connection.Close();
                Connections.TryGetValue(name, out result);
            }
            return result;
        }

        private static string GetHost(ConnectionMultiplexer connection)
        {
            var result = new StringBuilder();
            foreach (var ep in connection.GetEndPoints())
            {
                if (result.Length > 0) result.Append(",");
                result.Append(ep.ToString().Split(':')[0]);
            }

            return result.ToString();
        }
    }
}
