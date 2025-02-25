using MongoDB.Driver;
using System;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Core.Commands
{
    class ActionCommand<TDomain> : ICommand
    {
        private readonly IMongoCollection<TDomain> Collection;

        private readonly Func<IMongoCollection<TDomain>, bool> Action;

        public ActionCommand(IMongoCollection<TDomain> collection, Func<IMongoCollection<TDomain>, bool> action)
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
