using MongoDB.Driver;
using System;
using System.Linq.Expressions;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Core.Commands
{
    class ReplaceCommand<TDomain, TKey> : ICommand
    {
        private readonly IMongoCollection<TDomain> Collection;

        private readonly TKey Id;

        private readonly TDomain Domain;

        private readonly DbSet<TDomain, TKey> DbSet;

        public ReplaceCommand(IMongoCollection<TDomain> collection, TKey id, TDomain domain, DbSet<TDomain, TKey> dbSet)
        {
            Id = id;
            Collection = collection;
            Domain = domain;
            DbSet = dbSet;
        }

        public bool Execute()
        {
            var type = typeof(TDomain);
            var source = DbSet.IdentityMap.Get(Id);
            if (source == null)
            {
                throw new ClassicDomainException(type, "修改的实例必须经过加载。");
            }
            var context = Library.UpdateContext<TDomain>.GetContext(Id, type, source, Domain);
            if (!context.HasValue()) return false;

            try
            {
                var result = context.Execute(Collection);
                if (result) DbSet.IdentityMap.Set(Domain);
                return result;
            }
            catch (MongoDuplicateKeyException ex)
            {
                throw new UniqueException(type, ex);
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
    }
}
