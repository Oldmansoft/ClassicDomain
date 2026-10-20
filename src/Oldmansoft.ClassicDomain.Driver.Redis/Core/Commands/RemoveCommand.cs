using Oldmansoft.ClassicDomain.Driver.Redis.Library;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Core.Commands
{
    class RemoveCommand<TDomain, TKey> : ICommand
    {
        private readonly IDatabase Db;

        private readonly Func<TKey, string> MergeKey;

        private readonly IMergeKey<TKey> Merge;

        private readonly IdentityMap<TDomain> IdentityMap;

        private readonly TKey Key;

        public RemoveCommand(IDatabase db, Func<TKey, string> mergeKey, IMergeKey<TKey> merge, IdentityMap<TDomain> identityMap, TKey key)
        {
            Db = db;
            MergeKey = mergeKey;
            Merge = merge;
            IdentityMap = identityMap;
            Key = key;
        }

        public bool Execute()
        {
            var command = GetContext(Key, typeof(TDomain));
            var batch = Db.CreateBatch();
            var tasks = new List<Task>();
            command.Queue(batch, Merge, tasks);
            var deleted = batch.KeyDeleteAsync(MergeKey(command.Key));
            tasks.Add(deleted);
            batch.Execute();
            command.Wait(tasks);
            if (!deleted.Result) return false;
            IdentityMap.Remove(command.Key);
            return true;
        }

        /// <summary>
        /// 获取移除项
        /// </summary>
        /// <param name="key"></param>
        /// <param name="domainType"></param>
        /// <returns></returns>
        private UpdatedCommand<TKey> GetContext(TKey key, Type domainType)
        {
            var result = new UpdatedCommand<TKey>(key, domainType);
            var reflection = ContextGetHelper.GetReflection(domainType);
            foreach (var name in reflection.ListNames) result.KeyDelete.Add(name);
            foreach (var name in reflection.HashNames) result.KeyDelete.Add(name);
            return result;
        }
    }
}
