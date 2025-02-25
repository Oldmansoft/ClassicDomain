using MongoDB.Driver;
using System;
using System.Linq.Expressions;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Core.Commands
{
    class RemoveCommand<TDomain, TKey> : ICommand
    {
        private readonly IMongoCollection<TDomain> Collection;

        private readonly TKey Id;

        private readonly DbSet<TDomain, TKey> DbSet;

        public RemoveCommand(IMongoCollection<TDomain> collection, TKey id, DbSet<TDomain, TKey> dbSet)
        {
            Collection = collection;
            Id = id;
            DbSet = dbSet;
        }

        public bool Execute()
        {
            var result = ExecuteRemove();
            if (result) DbSet.IdentityMap.Remove(Id);
            return result;
        }

        private bool ExecuteRemove()
        {
            var filter = Builders<TDomain>.Filter.Eq(DbSet.KeyExpression, Id);
            Collection.DeleteOne(filter);
            return true;
        }
    }
}
