using Oldmansoft.ClassicDomain.Driver.Redis.Library;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Core.Commands
{
    class SafeModeRemoveCommand<TDomain, TKey> : ICommand
    {
        private IDatabase Db;

        private Func<TKey, string> MergeKey;

        private IMergeKey<TKey> Merge;

        private IdentityMap<TDomain> IdentityMap;

        private TKey Key;

        public SafeModeRemoveCommand(IDatabase db, Func<TKey, string> mergeKey, IMergeKey<TKey> merge, IdentityMap<TDomain> identityMap, TKey key)
        {
            Db = db;
            MergeKey = mergeKey;
            Merge = merge;
            IdentityMap = identityMap;
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
            var command = ContextSetRemoveHelper.GetContext(Key, typeof(TDomain));
            command.Execute(Db, Merge);
            if (!Db.KeyDelete(MergeKey(command.Key))) return false;
            IdentityMap.Remove(command.Key);
            return true;
        }
    }
}
