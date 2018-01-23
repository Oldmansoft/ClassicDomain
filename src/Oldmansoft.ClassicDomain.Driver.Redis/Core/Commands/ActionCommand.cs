using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Core.Commands
{
    class ActionCommand<TDomain> : ICommand
    {
        private IDatabase Db;

        private Func<IDatabase, bool> Action;

        public ActionCommand(IDatabase db, Func<IDatabase, bool> action)
        {
            Db = db;
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
            return Action(Db);
        }
    }
}
