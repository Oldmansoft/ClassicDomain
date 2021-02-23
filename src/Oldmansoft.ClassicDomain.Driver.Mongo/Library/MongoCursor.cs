using System;
using System.Collections.Generic;
using System.Text;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Library
{
    internal class MongoCursor<TDocument> : MongoDB.Driver.MongoCursor<TDocument> where TDocument : class
    {
        private IdentityMap<TDocument> IdentityMap { get; set; }

        public MongoCursor(
            MongoDB.Driver.MongoCollection collection,
            MongoDB.Driver.IMongoQuery query,
            MongoDB.Driver.ReadConcern readConcern,
            MongoDB.Driver.ReadPreference readPreference,
            MongoDB.Bson.Serialization.IBsonSerializer serializer,
            IdentityMap<TDocument> identityMap
        )
            : base(collection, query, readConcern, readPreference, serializer)
        {
            IdentityMap = identityMap;
        }

        public override IEnumerator<TDocument> GetEnumerator()
        {
            return new MongoCursorEnumerator<TDocument>(base.GetEnumerator(), IdentityMap);
        }
    }
}
