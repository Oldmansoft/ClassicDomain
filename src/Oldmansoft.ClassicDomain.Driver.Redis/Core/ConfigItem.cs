using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Core
{
    /// <summary>
    /// 配置项
    /// </summary>
    class ConfigItem
    {
        /// <summary>
        /// 服务
        /// </summary>
        private ConnectionMultiplexer Connection { get; set; }

        /// <summary>
        /// 主机名
        /// </summary>
        public string Host { get; private set; }

        /// <summary>
        /// 是否低服务器版本
        /// </summary>
        public bool IsLowServerVersion { get; private set; }

        /// <summary>
        /// 创建配置项
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="host"></param>
        /// <param name="serverVersion"></param>
        public ConfigItem(ConnectionMultiplexer connection, string host, string serverVersion)
        {
            Connection = connection;
            Host = host;
            IsLowServerVersion = serverVersion == "2";
        }

        /// <summary>
        /// 获取数据库
        /// </summary>
        /// <returns></returns>
        public IDatabase GetDatabase()
        {
            return Connection.GetDatabase();
        }
    }
}
