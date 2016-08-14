using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Oldmansoft.ClassicDomain.Driver.Mongo
{
    /// <summary>
    /// 快速模式配置
    /// </summary>
    internal class FastModeConfig
    {
        private Dictionary<string, ConfigItem> Items { get; set; }

        /// <summary>
        /// 配置
        /// </summary>
        public FastModeConfig()
        {
            Items = new Dictionary<string, ConfigItem>();
        }

        private ConfigItem InitItem(string name)
        {
            MongoServerSettings setting = new MongoServerSettings();
            var connectionString = Configuration.Config.GetConnectionString(name, 27017);
            setting.Server = new MongoServerAddress(connectionString.DataSource.Host, connectionString.DataSource.Port);
            setting.WriteConcern = WriteConcern.Acknowledged;
            if (!string.IsNullOrEmpty(connectionString.UserID))
            {
                setting.Credentials = new[] { MongoCredential.CreateMongoCRCredential(connectionString.InitialCatalog, connectionString.UserID, connectionString.Password) };
            }

            return new ConfigItem(CreateMongoServer(setting), connectionString.InitialCatalog);
        }

        /// <summary>
        /// 获取数据库
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public MongoDatabase GetDatabase(string name)
        {
            if (Items.ContainsKey(name))
            {
                return Items[name].GetDatabase();
            }

            ConfigItem item = InitItem(name);
            lock (Items)
            {
                if (Items.ContainsKey(name))
                {
                    return Items[name].GetDatabase();
                }

                Items.Add(name, item);
                return item.GetDatabase();
            }
        }

        /// <summary>
        /// 创建服务器
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        protected virtual MongoServer CreateMongoServer(MongoServerSettings setting)
        {
            return new MongoServer(setting);
        }
    }
}
