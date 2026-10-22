using StackExchange.Redis;
using System;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Core.Commands
{
    class ActionCommand<TDomain> : ICommand
    {
        private readonly IDatabase Db;

        private readonly Func<IDatabase, bool> Action;

        private readonly IRedisFlush Flush;

        public ActionCommand(IDatabase db, Func<IDatabase, bool> action, IRedisFlush flush)
        {
            Db = db;
            Action = action;
            Flush = flush;
        }

        public bool Execute()
        {
            Flush.Send();
            return Action(Db);
        }
    }
}
