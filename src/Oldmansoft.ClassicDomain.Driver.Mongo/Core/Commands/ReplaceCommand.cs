using MongoDB.Driver;
using System;
using System.Linq.Expressions;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Core.Commands
{
    class ReplaceCommand<TDomain, TKey> : ICommand
    {
        private readonly MongoCollection<TDomain> Collection;

        private readonly TKey Id;

        private readonly TDomain Domain;

        private readonly IdentityMap<TDomain> IdentityMap;

        public ReplaceCommand(MongoCollection<TDomain> collection, TKey id, TDomain domain, IdentityMap<TDomain> identityMap)
        {
            Id = id;
            Collection = collection;
            Domain = domain;
            IdentityMap = identityMap;
        }

        public bool Execute()
        {
            var type = typeof(TDomain);
            var source = IdentityMap.Get(Id);
            if (source == null)
            {
                throw new ClassicDomainException(type, "修改的实例必须经过加载。");
            }
            var context = Library.UpdateContext.GetContext(Id, type, source, Domain);
            if (!context.HasValue()) return false;

            try
            {
                var result = context.Execute(Collection);
                if (result) IdentityMap.Set(Domain);
                return result;
            }
            catch (MongoDuplicateKeyException ex)
            {
                throw new UniqueException(type, ex);
            }
        }
    }
}
