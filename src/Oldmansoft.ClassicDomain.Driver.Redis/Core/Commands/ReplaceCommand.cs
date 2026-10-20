using Oldmansoft.ClassicDomain.Driver.Redis.Library;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            var transaction = Db.CreateTransaction();
            transaction.AddCondition(Condition.HashExists(MergeKey(command.Key), "this"));
            var tasks = new List<Task>();
            command.Queue(transaction, Merge, tasks);
            if (!transaction.Execute()) return false;
            command.Wait(tasks);
            IdentityMap.Set(Domain);
            return true;
        }
    }
}
