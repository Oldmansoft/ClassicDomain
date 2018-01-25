using Oldmansoft.ClassicDomain.Driver.Redis.Library;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Core.Commands
{
    class SafeModeReplaceCommand<TDomain, TKey> : ICommand
    {
        private IDatabase Db;

        private Func<TKey, string> MergeKey;

        private IMergeKey<TKey> Merge;

        private IdentityMap<TDomain> IdentityMap;

        private TKey Key;

        private TDomain Domain;

        public SafeModeReplaceCommand(IDatabase db, Func<TKey, string> mergeKey, IMergeKey<TKey> merge, IdentityMap<TDomain> identityMap, TKey key, TDomain domain)
        {
            Db = db;
            MergeKey = mergeKey;
            Merge = merge;
            IdentityMap = identityMap;
            Key = key;
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
            var source = IdentityMap.Get(Key);
            if (source == null)
            {
                throw new ClassicDomainException(Type, "修改的实例必须经过加载");
            }
            var command = ContextSetReplaceHelper.GetContext(Key, typeof(TDomain), source, Domain);
            if (string.IsNullOrEmpty(Db.HashGet(MergeKey(command.Key), "this"))) return false;
            command.Execute(Db, Merge);
            IdentityMap.Set(Domain);
            return true;
        }
    }
}
