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
            var models = new List<WriteModel<TDomain>>();
            if (NormalUpdate != null)
            {
                models.Add(new UpdateOneModel<TDomain>(Filter, NormalUpdate));
            }
            foreach (var update in UpdateList)
            {
                models.Add(new UpdateOneModel<TDomain>(Filter, update));
            }
            if (models.Count == 0) return false;

            var writeResult = collection.BulkWrite(models, new BulkWriteOptions { IsOrdered = true });
            return writeResult != null && writeResult.ModifiedCount > 0;
        }
    }
}
