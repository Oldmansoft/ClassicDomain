using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Core
{
    /// <summary>
    /// 配置项
    /// </summary>
    class Config
    {
        /// <summary>
        /// 服务
        /// </summary>
        private ConnectionMultiplexer Connection { get; set; }

        /// <summary>
        /// 是否低服务器版本
        /// </summary>
        public bool IsLowServerVersion { get; private set; }

        /// <summary>
        /// 创建配置项
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="isLowServerVersion"></param>
        public Config(ConnectionMultiplexer connection, bool isLowServerVersion)
        {
            Connection = connection;
            IsLowServerVersion = isLowServerVersion;
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
