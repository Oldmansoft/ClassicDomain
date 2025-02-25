using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Library
{
    internal class UpdatedItem<TDomain>
    {
        private readonly FilterDefinition<TDomain> Filter;

        private UpdateDefinition<TDomain> NormalUpdate;

        private readonly List<UpdateDefinition<TDomain>> UpdateList;

        public UpdatedItem(BsonValue id)
        {
            Filter = Builders<TDomain>.Filter.Eq("_id", id);
            UpdateList = new List<UpdateDefinition<TDomain>>();
        }

        public void Combine(UpdateDefinition<TDomain> update)
        {
            if (NormalUpdate == null)
            {
                NormalUpdate = update;
            }
            else
            {
                NormalUpdate = Builders<TDomain>.Update.Combine(NormalUpdate, update);
            }
        }

        public void AddOne(UpdateDefinition<TDomain> update)
        {
            UpdateList.Add(update);
        }

        public bool HasValue()
        {
            return NormalUpdate != null || UpdateList.Count > 0;
        }

        public bool Execute(IMongoCollection<TDomain> collection)
        {
            bool result = false;
            if (NormalUpdate != null)
            {
                var writeResult = collection.UpdateOne(Filter, NormalUpdate);
                if (writeResult == null)
                {
                    result = true;
                }
                else
                {
                    result = writeResult.ModifiedCount > 0;
                }
            }
            foreach (var update in UpdateList)
            {
                var writeResult = collection.UpdateOne(Filter, update);
                if (writeResult == null)
                {
                    result = true;
                }
                else
                {
                    result = writeResult.ModifiedCount > 0 || result;
                }
            }
            return result;
        }
    }
}
