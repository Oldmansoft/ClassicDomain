using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver.Builders;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Library
{
    static class Extend
    {
        public static UpdateBuilder SetObjectWrapped(this UpdateBuilder source, string name, object value)
        {
            if (value == null) return Update.Set(name, BsonValue.Create(null));
            if (name == null) { throw new ArgumentNullException("name"); }

            var wrappedValue = BsonDocumentWrapper.Create(value.GetType(), value);
            BsonElement element;
            var document = source.ToBsonDocument();
            if (document.TryGetElement("$set", out element))
            {
                element.Value.AsBsonDocument.Add(name, wrappedValue);
            }
            else
            {
                document.Add("$set", new BsonDocument(name, wrappedValue));
            }
            return source;
        }

        public static UpdateBuilder PullObjectWrapped(this UpdateBuilder source, string name, object value)
        {
            if (value == null) return Update.Pull(name, BsonValue.Create(null));
            if (name == null) { throw new ArgumentNullException("name"); }

            var wrappedValue = BsonDocumentWrapper.Create(value.GetType(), value);
            BsonElement element;
            var document = source.ToBsonDocument();
            if (document.TryGetElement("$pull", out element))
            {
                element.Value.AsBsonDocument.Add(name, wrappedValue);
            }
            else
            {
                document.Add("$pull", new BsonDocument(name, wrappedValue));
            }
            return source;
        }
        
        public static BsonValue ToBsonValue(this object source)
        {
            if (source is decimal)
            {
                return BsonValue.Create(source.ToString());
            }
            else
            {
                return BsonValue.Create(source);
            }
        }
    }
}
