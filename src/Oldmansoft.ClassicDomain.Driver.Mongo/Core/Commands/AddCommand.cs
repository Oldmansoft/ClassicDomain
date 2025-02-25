using MongoDB.Driver;
using System;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Core.Commands
{
    class AddCommand<TDomain> : ICommand
    {
        private readonly IMongoCollection<TDomain> Collection;

        private readonly TDomain Domain;

        private readonly IdentityMap<TDomain> IdentityMap;

        public AddCommand(IMongoCollection<TDomain> collection, TDomain domain, IdentityMap<TDomain> identityMap)
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
            catch (MongoWriteException ex)
            {
                if (ex.WriteError.Code == 11000)
                {
                    throw new UniqueException(typeof(TDomain), ex.WriteError.Message);
                }
                else
                {
                    throw new ClassicDomainException(typeof(TDomain), ex.Message);
                }
            }
        }

        private bool ExecuteInsert()
        {
            Collection.InsertOne(Domain);
            return true;
        }
    }
}
