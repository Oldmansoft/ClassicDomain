using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
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
            var databaseName = new Uri(connectionString).GetDatabaseName();
            if (string.IsNullOrEmpty(databaseName))
            {
                databaseName = "DefaultDatabase";
            }
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
            return new Config(new MongoClient(new MongoUrl(connectionString)), databaseName);
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
    }
}
