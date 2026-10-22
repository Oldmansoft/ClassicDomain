using MongoDB.Bson;
using System.Collections.Generic;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Library
{
    interface IUpdateBuffer
    {
        void Set(string path, BsonValue value);

        void Unset(string path);

        void PushEach<TItem>(string path, IEnumerable<TItem> values);

        void PopLast(string path);

        void SetArray(string path, BsonArray value);

        void SetValue<TItem>(string path, TItem value);
    }
}
