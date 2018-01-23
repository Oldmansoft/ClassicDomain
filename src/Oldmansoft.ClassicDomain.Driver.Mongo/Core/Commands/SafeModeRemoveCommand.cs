using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Core.Commands
{
    class SafeModeRemoveCommand<TDomain, TKey> : RemoveCommand<TDomain, TKey>
    {
        private IdentityMap<TDomain> IdentityMap;

        public SafeModeRemoveCommand(System.Linq.Expressions.Expression<Func<TDomain, TKey>> keyExpression,
            Func<TDomain, TKey> keyExpressionCompile,
            MongoCollection<TDomain> collection,
            TDomain domain,
            IdentityMap<TDomain> identityMap)
            : base(keyExpression, keyExpressionCompile, collection, domain)
        {
            IdentityMap = identityMap;
        }

        public override bool Execute()
        {
            var result = base.Execute();
            if (result) IdentityMap.Remove(KeyExpressionCompile(Domain));
            return result;
        }
    }
}
