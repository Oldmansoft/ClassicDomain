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
            MongoCollection<TDomain> collection,
            TKey id,
            IdentityMap<TDomain> identityMap)
            : base(keyExpression, collection, id)
        {
            IdentityMap = identityMap;
        }

        public override bool Execute()
        {
            var result = base.Execute();
            if (result) IdentityMap.Remove(Id);
            return result;
        }
    }
}
