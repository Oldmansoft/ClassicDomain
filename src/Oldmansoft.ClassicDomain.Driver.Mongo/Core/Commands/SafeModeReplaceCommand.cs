using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Core.Commands
{
    class SafeModeReplaceCommand<TDomain, TKey> : ICommand
    {
        private System.Linq.Expressions.Expression<Func<TDomain, TKey>> KeyExpression;

        private Func<TDomain, TKey> KeyExpressionCompile;

        private MongoCollection<TDomain> Collection;

        private TDomain Domain;

        private IdentityMap<TDomain> IdentityMap;

        public SafeModeReplaceCommand(System.Linq.Expressions.Expression<Func<TDomain, TKey>> keyExpression,
            Func<TDomain, TKey> keyExpressionCompile,
            MongoCollection<TDomain> collection,
            TDomain domain,
            IdentityMap<TDomain> identityMap)
        {
            KeyExpression = keyExpression;
            KeyExpressionCompile = keyExpressionCompile;
            Collection = collection;
            Domain = domain;
            IdentityMap = identityMap;
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
            var id = KeyExpressionCompile(Domain);
            var source = IdentityMap.Get(id);
            if (source == null)
            {
                throw new ClassicDomainException(Type, "修改的实例必须经过加载。");
            }
            var context = Library.UpdateContext.GetContext(id, Type, source, Domain);
            if (!context.HasValue()) return false;

            try
            {
                var result = context.Execute(Collection);
                if (result) IdentityMap.Set(Domain);
                return result;
            }
            catch (MongoDuplicateKeyException ex)
            {
                throw new UniqueException(Type, ex);
            }
        }
    }
}
