using StackExchange.Redis;
using System;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Core.Commands
{
    class ActionCommand<TDomain> : ICommand
    {
        private readonly IDatabase Db;

        private readonly Func<IDatabase, bool> Action;

        public ActionCommand(IDatabase db, Func<IDatabase, bool> action)
        {
            Db = db;
            Action = action;
        }

        public bool Execute()
        {
            return Action(Db);
        }
    }
}
