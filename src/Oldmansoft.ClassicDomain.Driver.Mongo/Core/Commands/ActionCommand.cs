using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Core.Commands
{
    class ActionCommand<TDomain> : ICommand
    {
        private MongoCollection<TDomain> Collection;

        private Func<MongoCollection<TDomain>, bool> Action;

        public ActionCommand(MongoCollection<TDomain> collection, Func<MongoCollection<TDomain>, bool> action)
        {
            Collection = collection;
            Action = action;
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
            return Action(Collection);
        }
    }
}
