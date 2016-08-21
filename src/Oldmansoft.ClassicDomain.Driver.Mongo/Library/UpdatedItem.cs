using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Library
{
    internal class UpdatedItem
    {
        private IMongoQuery Query { get; set; }

        private IMongoUpdate NormalUpdate { get; set; }

        private List<IMongoUpdate> UpdateList { get; set; }

        public UpdatedItem(BsonValue id)
        {
            Query = MongoDB.Driver.Builders.Query.EQ("_id", id);
            UpdateList = new List<IMongoUpdate>();
        }

        public void Add(IMongoUpdate update)
        {
            if (NormalUpdate == null)
            {
                NormalUpdate = update;
            }
            else
            {
                NormalUpdate = MongoDB.Driver.Builders.Update.Combine(NormalUpdate, update);
            }
        }

        public void AddOther(IMongoUpdate update)
        {
            UpdateList.Add(update);
        }

        public bool HasValue()
        {
            return NormalUpdate != null || UpdateList.Count > 0;
        }

        public bool Execute<TEntity>(MongoDB.Driver.MongoCollection<TEntity> collection)
        {
            bool result = false;
            if (NormalUpdate != null)
            {
                var writeResult = collection.Update(Query, NormalUpdate);
                if (writeResult == null)
                {
                    result = true;
                }
                else
                {
                    result = writeResult.DocumentsAffected > 0;
                }
            }
            foreach (var update in UpdateList)
            {
                var writeResult = collection.Update(Query, update);
                if (writeResult == null)
                {
                    result = true;
                }
                else
                {
                    result = writeResult.DocumentsAffected > 0 || result;
                }
            }
            return result;
        }
    }
}
