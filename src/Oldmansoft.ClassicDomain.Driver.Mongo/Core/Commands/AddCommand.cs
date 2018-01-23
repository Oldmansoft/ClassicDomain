using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Core.Commands
{
    class AddCommand<TDomain> : ICommand
    {
        private MongoCollection<TDomain> Collection;

        protected TDomain Domain { get; private set; }

        public AddCommand(MongoCollection<TDomain> collection, TDomain domain)
        {
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
            var result = Collection.Insert(Domain);
            if (result == null) return true;
            return !result.HasLastErrorMessage;
        }
    }
}
