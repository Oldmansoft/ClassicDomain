using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Library
{
    internal sealed class UpdateBuffer<TDomain> : IUpdateBuffer
    {
        private readonly List<UpdateDefinition<TDomain>> Parts = new List<UpdateDefinition<TDomain>>();

        public bool HasValue
        {
            get { return Parts.Count > 0; }
        }

        public void Set(string path, BsonValue value)
        {
            Parts.Add(Builders<TDomain>.Update.Set(path, value));
        }

        public void Unset(string path)
        {
            Parts.Add(Builders<TDomain>.Update.Unset(path));
        }

        public void PushEach<TItem>(string path, IEnumerable<TItem> values)
        {
            Parts.Add(Builders<TDomain>.Update.PushEach(path, values));
        }

        public void PopLast(string path)
        {
            Parts.Add(new BsonDocumentUpdateDefinition<TDomain>(new BsonDocument("$pop", new BsonDocument(path, 1))));
        }

        public void SetArray(string path, BsonArray value)
        {
            Parts.Add(Builders<TDomain>.Update.Set(path, value));
        }

        public void SetValue<TItem>(string path, TItem value)
        {
            Parts.Add(Builders<TDomain>.Update.Set(path, value));
        }

        public UpdateDefinition<TDomain> ToUpdate()
        {
            var result = Parts[0];
            for (var i = 1; i < Parts.Count; i++)
            {
                result = Builders<TDomain>.Update.Combine(result, Parts[i]);
            }
            return result;
        }
    }
}
