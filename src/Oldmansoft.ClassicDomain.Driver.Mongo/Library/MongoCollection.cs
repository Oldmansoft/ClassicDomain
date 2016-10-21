using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Library
{
    internal class MongoCollection<TDocument> : MongoDB.Driver.MongoCollection<TDocument>
    {
        private IdentityMap<TDocument> IdentityMap;

        public MongoCollection(
            MongoDatabase database,
            string name,
            MongoDB.Driver.MongoCollectionSettings settings,
            IdentityMap<TDocument> identityMap
        )
            : base(database, name, settings)
        {
            IdentityMap = identityMap;
        }

        public override TDocument FindOneById(BsonValue id)
        {
            var result = base.FindOneById(id);
            IdentityMap.Set(result);
            return result;
        }

        public override MongoDB.Driver.MongoCursor FindAs(Type documentType, MongoDB.Driver.IMongoQuery query)
        {
            return CreateMongoCursor(documentType, this, query, Settings.ReadConcern, Settings.ReadPreference, MongoDB.Bson.Serialization.BsonSerializer.LookupSerializer(documentType), IdentityMap);
        }

        private static MongoDB.Driver.MongoCursor CreateMongoCursor(Type documentType, MongoDB.Driver.MongoCollection collection, MongoDB.Driver.IMongoQuery query, MongoDB.Driver.ReadConcern readConcern, MongoDB.Driver.ReadPreference readPreference, MongoDB.Bson.Serialization.IBsonSerializer serializer, IdentityMap<TDocument> store)
        {
            var cursorDefinition = typeof(MongoCursor<>);
            var cursorType = cursorDefinition.MakeGenericType(documentType);
            var constructorInfo = cursorType.GetConstructor(
                new Type[] {
                    typeof(MongoDB.Driver.MongoCollection),
                    typeof(MongoDB.Driver.IMongoQuery),
                    typeof(MongoDB.Driver.ReadConcern),
                    typeof(MongoDB.Driver.ReadPreference),
                    typeof(MongoDB.Bson.Serialization.IBsonSerializer),
                    typeof(IdentityMap<TDocument>)
                }
            );
            return (MongoDB.Driver.MongoCursor)constructorInfo.Invoke(new object[] { collection, query, readConcern, readPreference, serializer, store });
        }
    }
}
