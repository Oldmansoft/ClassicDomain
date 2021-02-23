using MongoDB.Driver;
using System;
using System.Linq.Expressions;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Core.Commands
{
    class RemoveCommand<TDomain, TKey> : ICommand
    {
        private readonly MongoCollection<TDomain> Collection;

        private readonly Expression<Func<TDomain, TKey>> KeyExpression;

        private readonly TKey Id;

        private readonly IdentityMap<TDomain> IdentityMap;

        public RemoveCommand(MongoCollection<TDomain> collection, Expression<Func<TDomain, TKey>> keyExpression, TKey id, IdentityMap<TDomain> identityMap)
        {
            Collection = collection;
            KeyExpression = keyExpression;
            Id = id;
            IdentityMap = identityMap;
        }

        public bool Execute()
        {
            var result = ExecuteRemove();
            if (result) IdentityMap.Remove(Id);
            return result;
        }

        private bool ExecuteRemove()
        {
            var query = MongoDB.Driver.Builders.Query<TDomain>.EQ(KeyExpression, Id);
            var result = Collection.Remove(query);
            if (result == null) return true;
            return result.DocumentsAffected > 0;
        }
    }
}
