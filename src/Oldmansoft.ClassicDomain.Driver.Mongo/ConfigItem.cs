using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Oldmansoft.ClassicDomain.Driver.Mongo
{
    /// <summary>
    /// 配置项
    /// </summary>
    internal class ConfigItem
    {
        /// <summary>
        /// 服务
        /// </summary>
        private MongoServer Server { get; set; }

        /// <summary>
        /// 数据库名称
        /// </summary>
        private string DatabaseName { get; set; }

        /// <summary>
        /// 创建配置项
        /// </summary>
        /// <param name="server"></param>
        /// <param name="databaseName"></param>
        public ConfigItem(MongoServer server, string databaseName)
        {
            Server = server;
            DatabaseName = databaseName;
        }

        /// <summary>
        /// 获取数据库
        /// </summary>
        /// <returns></returns>
        public MongoDatabase GetDatabase()
        {
            return Server.GetDatabase(DatabaseName);
        }
    }
}
