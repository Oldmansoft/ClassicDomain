using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Core
{
    internal abstract class Config
    {
        private Dictionary<string, ConfigItem> Items { get; set; }

        /// <summary>
        /// 配置
        /// </summary>
        public Config()
        {
            Items = new Dictionary<string, ConfigItem>();
        }

        private ConfigItem InitItem(string name)
        {
            var connectionString = Configuration.Config.GetConnectionString(name);
            string database;
            MongoServerSettings setting;
            if (connectionString.IndexOf("mongodb://") > -1)
            {
                setting = MongoServerSettings.FromUrl(new MongoUrl(connectionString));
                database = new Uri(connectionString).GetDatabase();
            }
            else
            {
                var connectionContext = new Configuration.ConnectionString(name, connectionString, 27017);
                var urlBuilder = new MongoUrlBuilder();
                var servers = new List<MongoServerAddress>();
                foreach (var dataSource in connectionContext.DataSource)
                {
                    servers.Add(new MongoServerAddress(dataSource.Host, dataSource.Port));
                }
                urlBuilder.Servers = servers;
                if (!string.IsNullOrEmpty(connectionContext.UserID))
                {
                    urlBuilder.Username = connectionContext.UserID;
                    urlBuilder.Password = connectionContext.Password;
                }

                setting = MongoServerSettings.FromUrl(urlBuilder.ToMongoUrl());
                setting.WriteConcern = WriteConcern.Acknowledged;
                database = connectionContext.InitialCatalog;
            }
            if (string.IsNullOrEmpty(database))
            {
                throw new ConfigItemException(string.Format("config 文件的配置项 {0} ConnectionString 需要指定数据库名称", name));
            }
            return new ConfigItem(CreateMongoServer(setting), database);
        }

        /// <summary>
        /// 获取配置项
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ConfigItem Get(string name)
        {
            if (Items.ContainsKey(name))
            {
                return Items[name];
            }

            ConfigItem item = InitItem(name);
            lock (Items)
            {
                if (Items.ContainsKey(name))
                {
                    return Items[name];
                }

                Items.Add(name, item);
                return item;
            }
        }

        /// <summary>
        /// 创建服务器
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        protected abstract MongoServer CreateMongoServer(MongoServerSettings setting);
    }
}
