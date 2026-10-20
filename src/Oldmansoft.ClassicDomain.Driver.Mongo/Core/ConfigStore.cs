using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Core
{
    internal class ConfigStore
    {
        private static readonly object SerializerGate = new object();

        private static bool GuidSerializerRegistered;

        private Dictionary<string, Config> Items { get; set; }

        /// <summary>
        /// 配置
        /// </summary>
        public ConfigStore()
        {
            Items = new Dictionary<string, Config>();
        }

        private static void EnsureGuidSerializer()
        {
            if (GuidSerializerRegistered) return;
            lock (SerializerGate)
            {
                if (GuidSerializerRegistered) return;
                try
                {
#pragma warning disable CS0618
                    BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V3;
#pragma warning restore CS0618
                }
                catch (BsonSerializationException)
                {
                }
                try
                {
                    BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
                }
                catch (BsonSerializationException)
                {
                }
                GuidSerializerRegistered = true;
            }
        }

        private Config InitItem(Type callerType)
        {
            var connectionString = ConnectionString.Get(callerType);
            var databaseName = new Uri(connectionString).GetDatabaseName();
            if (string.IsNullOrEmpty(databaseName))
            {
                databaseName = "DefaultDatabase";
            }
            var settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
#pragma warning disable CS0618
            settings.LinqProvider = LinqProvider.V3;
#pragma warning restore CS0618
            return new Config(new MongoClient(settings), databaseName);
        }

        /// <summary>
        /// 获取配置项
        /// </summary>
        /// <param name="callerType"></param>
        /// <returns></returns>
        public Config Get(Type callerType)
        {
            var name = callerType.FullName;
            lock (Items)
            {
                if (Items.TryGetValue(name, out Config existing))
                {
                    return existing;
                }

                EnsureGuidSerializer();
                var item = InitItem(callerType);
                Items.Add(name, item);
                return item;
            }
        }
    }
}
