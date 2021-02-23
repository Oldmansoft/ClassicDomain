using Oldmansoft.ClassicDomain.Driver.Redis.Library;
using StackExchange.Redis;
using System;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Core.Commands
{
    class ReplaceCommand<TDomain, TKey> : ICommand
    {
        private readonly IDatabase Db;

        private readonly Func<TKey, string> MergeKey;

        private readonly IMergeKey<TKey> Merge;

        private readonly IdentityMap<TDomain> IdentityMap;

        private readonly TKey Key;

        private readonly TDomain Domain;

        public ReplaceCommand(IDatabase db, Func<TKey, string> mergeKey, IMergeKey<TKey> merge, IdentityMap<TDomain> identityMap, TKey key, TDomain domain)
        {
            Db = db;
            MergeKey = mergeKey;
            Merge = merge;
            IdentityMap = identityMap;
            Key = key;
            Domain = domain;
        }

        public bool Execute()
        {
            var type = typeof(TDomain);
            var source = IdentityMap.Get(Key);
            if (source == null)
            {
                throw new ClassicDomainException(type, "修改的实例必须经过加载");
            }
            var command = ContextReplaceHelper.GetContext(Key, type, source, Domain);
            if (string.IsNullOrEmpty(Db.HashGet(MergeKey(command.Key), "this"))) return false;
            command.Execute(Db, Merge);
            IdentityMap.Set(Domain);
            return true;
        }
    }
}
