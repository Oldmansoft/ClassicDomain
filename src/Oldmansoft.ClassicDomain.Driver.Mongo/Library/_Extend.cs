using MongoDB.Driver;
using MongoDB.Bson;
using System;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Library
{
    static class Extend
    {
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
