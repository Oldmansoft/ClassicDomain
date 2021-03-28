using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Core
{
    internal class ConfigStore
    {
        private Dictionary<string, Config> Items { get; set; }

        /// <summary>
        /// 配置
        /// </summary>
        public ConfigStore()
        {
            Items = new Dictionary<string, Config>();
        }

        private Config InitItem(Type callerType)
        {
            var connectionString = ConnectionString.Get(callerType);
            var setting = MongoServerSettings.FromUrl(new MongoUrl(connectionString));
            var databaseName = new Uri(connectionString).GetDatabaseName();

            if (string.IsNullOrEmpty(databaseName))
            {
                databaseName = "DefaultDatabase";
            }
            return new Config(CreateMongoServer(setting), databaseName);
        }

        /// <summary>
        /// 获取配置项
        /// </summary>
        /// <param name="callerType"></param>
        /// <returns></returns>
        public Config Get(Type callerType)
        {
            var name = callerType.FullName;
            if (Items.ContainsKey(name))
            {
                return Items[name];
            }

            var item = InitItem(callerType);
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
        private MongoServer CreateMongoServer(MongoServerSettings setting)
        {
            return new Library.MongoServer(setting);
        }
    }
}
