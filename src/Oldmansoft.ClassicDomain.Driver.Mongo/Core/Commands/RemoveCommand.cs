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

        protected Func<TDomain, TKey> KeyExpressionCompile;

        private MongoCollection<TDomain> Collection;

        protected TDomain Domain;

        public RemoveCommand(System.Linq.Expressions.Expression<Func<TDomain, TKey>> keyExpression, Func<TDomain, TKey> keyExpressionCompile, MongoCollection<TDomain> collection, TDomain domain)
        {
            KeyExpression = keyExpression;
            KeyExpressionCompile = keyExpressionCompile;
            Collection = collection;
            Domain = domain;
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
            var id = KeyExpressionCompile(Domain);
            var query = MongoDB.Driver.Builders.Query<TDomain>.EQ(KeyExpression, id);
            var result = Collection.Remove(query);
            if (result == null) return true;
            return result.DocumentsAffected > 0;
        }
    }
}
