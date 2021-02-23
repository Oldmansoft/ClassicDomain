using MongoDB.Driver;
using System;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Core.Commands
{
    class ActionCommand<TDomain> : ICommand
    {
        private readonly MongoCollection<TDomain> Collection;

        private readonly Func<MongoCollection<TDomain>, bool> Action;

        public ActionCommand(MongoCollection<TDomain> collection, Func<MongoCollection<TDomain>, bool> action)
        {
            Collection = collection;
            Action = action;
        }

        public bool Execute()
        {
            return Action(Collection);
        }
    }
}
