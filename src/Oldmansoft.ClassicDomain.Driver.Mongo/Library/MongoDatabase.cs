using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Library
{
    internal class MongoDatabase : MongoDB.Driver.MongoDatabase
    {
        private object IdentityMap { get; set; }

        public MongoDatabase(MongoDB.Driver.MongoServer server, string name, MongoDB.Driver.MongoDatabaseSettings settings)
            : base(server, name, settings)
        {
        }

        public void SetIdentityMap<TDocument>(IdentityMap<TDocument> identityMap)
        {
            IdentityMap = identityMap;
        }

        public override MongoDB.Driver.MongoCollection<TDefaultDocument> GetCollection<TDefaultDocument>(string collectionName, MongoDB.Driver.MongoCollectionSettings collectionSettings)
        {
            return new MongoCollection<TDefaultDocument>(this, collectionName, collectionSettings, IdentityMap as IdentityMap<TDefaultDocument>);
        }
    }
}
