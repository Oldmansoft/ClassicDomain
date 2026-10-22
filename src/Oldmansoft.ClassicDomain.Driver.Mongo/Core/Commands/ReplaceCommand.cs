using MongoDB.Driver;
using System;
using System.Linq;
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
                Queue(new ReplaceOneModel<TDomain>(Builders<TDomain>.Filter.Eq(DbSet.KeyExpression, Id), Domain) { IsUpsert = false });
                return true;
            }

            var buffer = new Library.UpdateBuffer<TDomain>();
            if (!Library.DiffCompiler<TDomain>.TryApply(source, Domain, buffer))
            {
                return ExecuteReflection(type, source);
            }
            if (!buffer.HasValue) return false;
            Queue(new UpdateOneModel<TDomain>(Builders<TDomain>.Filter.Eq(DbSet.KeyExpression, Id), buffer.ToUpdate()));
            return true;
        }

        bool ExecuteReflection(Type type, TDomain source)
        {
            var context = Library.UpdateContext<TDomain>.GetContext(Id, type, source, Domain);
            if (!context.HasValue()) return false;
            try
            {
                var result = context.Execute(Collection);
                if (result) DbSet.IdentityMap.SetReference(Domain);
                return result;
            }
            catch (MongoDuplicateKeyException ex)
            {
                throw new UniqueException(type, ex);
            }
            catch (MongoWriteException ex)
            {
                throw WriteError(ex.WriteError.Code, ex.WriteError.Message, ex.Message);
            }
            catch (MongoBulkWriteException<TDomain> ex)
            {
                var error = ex.WriteErrors.FirstOrDefault();
                if (error == null) throw new ClassicDomainException(type, ex.Message);
                throw WriteError(error.Code, error.Message, ex.Message);
            }
        }

        void Queue(WriteModel<TDomain> model)
        {
            DbSet.QueueWrite(model, Domain);
        }

        private Exception WriteError(int code, string writeMessage, string fallback)
        {
            if (code == 11000) return new UniqueException(typeof(TDomain), writeMessage);
            return new ClassicDomainException(typeof(TDomain), fallback);
        }
    }
}
