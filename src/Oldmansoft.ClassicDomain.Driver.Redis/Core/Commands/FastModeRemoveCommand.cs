using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Core.Commands
{
    class FastModeRemoveCommand<TDomain> : ICommand
    {
        private IDatabase Db;

        private string Key;

        public FastModeRemoveCommand(IDatabase db, string key)
        {
            Db = db;
            Key = key;
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
            return Db.KeyDelete(Key);
        }
    }
}
