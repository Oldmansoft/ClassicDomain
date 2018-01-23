using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Core.Commands
{
    class SafeModeAddCommand<TDomain> : AddCommand<TDomain>
    {
        private IdentityMap<TDomain> IdentityMap;

        public SafeModeAddCommand(MongoCollection<TDomain> collection, TDomain domain, IdentityMap<TDomain> identityMap)
            : base(collection, domain)
        {
            IdentityMap = identityMap;
        }

        public override bool Execute()
        {
            var result = base.Execute();
            if (result) IdentityMap.Set(Domain);
            return result;
        }
    }
}
