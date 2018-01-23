using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Core.Commands
{
    class FastModeAddCommand<TDomain> : ICommand
    {
        private IDatabase Db;

        private Func<TDomain, string> GetKey;

        private TDomain Domain;

        public FastModeAddCommand(IDatabase db, Func<TDomain, string> getKey, TDomain domain)
        {
            Db = db;
            GetKey = getKey;
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
            return Db.StringSet(GetKey(Domain), Library.Serializer.Serialize(Domain), null, When.NotExists);
        }
    }
}
