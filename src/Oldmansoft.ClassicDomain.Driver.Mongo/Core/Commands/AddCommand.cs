using MongoDB.Driver;
using System;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Core.Commands
{
    class AddCommand<TDomain> : ICommand
    {
        private readonly MongoCollection<TDomain> Collection;

        private readonly TDomain Domain;

        private readonly IdentityMap<TDomain> IdentityMap;

        public AddCommand(MongoCollection<TDomain> collection, TDomain domain, IdentityMap<TDomain> identityMap)
        {
            Collection = collection;
            Domain = domain;
            IdentityMap = identityMap;
        }

        public bool Execute()
        {
            try
            {
                var result = ExecuteInsert();
                if (result) IdentityMap.Set(Domain);
                return result;
            }
            catch (MongoDuplicateKeyException ex)
            {
                throw new UniqueException(typeof(TDomain), ex);
            }
        }

        private bool ExecuteInsert()
        {
            var result = Collection.Insert(Domain);
            if (result == null) return true;
            return !result.HasLastErrorMessage;
        }
    }
}
