using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Core.Commands
{
    class RemoveCommand<TDomain, TKey> : ICommand
    {
        private System.Linq.Expressions.Expression<Func<TDomain, TKey>> KeyExpression;
        
        private MongoCollection<TDomain> Collection;

        protected TKey Id;

        public RemoveCommand(System.Linq.Expressions.Expression<Func<TDomain, TKey>> keyExpression, MongoCollection<TDomain> collection, TKey id)
        {
            KeyExpression = keyExpression;
            Collection = collection;
            Id = id;
        }

        public Type Type
        {
            get
            {
                return typeof(TDomain);
            }
        }

        public virtual bool Execute()
        {
            var query = MongoDB.Driver.Builders.Query<TDomain>.EQ(KeyExpression, Id);
            var result = Collection.Remove(query);
            if (result == null) return true;
            return result.DocumentsAffected > 0;
        }
    }
}
