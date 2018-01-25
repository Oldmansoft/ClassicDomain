using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Core.Commands
{
    class FastModeReplaceCommand<TDomain, TKey> : ICommand
    {
        private System.Linq.Expressions.Expression<Func<TDomain, TKey>> KeyExpression;

        private Func<TDomain, TKey> KeyExpressionCompile;

        private MongoCollection<TDomain> Collection;

        private TDomain Domain;

        public FastModeReplaceCommand(System.Linq.Expressions.Expression<Func<TDomain, TKey>> keyExpression, Func<TDomain, TKey> keyExpressionCompile, MongoCollection<TDomain> collection, TDomain domain)
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

        public bool Execute()
        {
            var query = MongoDB.Driver.Builders.Query<TDomain>.EQ(KeyExpression, KeyExpressionCompile(Domain));
            var update = MongoDB.Driver.Builders.Update<TDomain>.Replace(Domain);

            try
            {
                var result = Collection.Update(query, update);
                if (result == null) return true;
                return result.DocumentsAffected > 0;
            }
            catch (MongoDuplicateKeyException ex)
            {
                throw new UniqueException(Type, ex);
            }
        }
    }
}
