using Oldmansoft.ClassicDomain.Driver.Redis.Library;
using System;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Core.Commands
{
    class RemoveCommand<TDomain, TKey> : ICommand
    {
        private readonly IMergeKey<TKey> Merge;

        private readonly IdentityMap<TDomain> IdentityMap;

        private readonly IRedisFlush Flush;

        private readonly TKey Key;

        public RemoveCommand(IMergeKey<TKey> merge, IdentityMap<TDomain> identityMap, IRedisFlush flush, TKey key)
        {
            Merge = merge;
            IdentityMap = identityMap;
            Flush = flush;
            Key = key;
        }

        public bool Execute()
        {
            var type = typeof(TDomain);
            var command = GetContext(Key, type);
            var previous = IdentityMap.Get(Key);
            IdentityMap.Remove(Key);
            Flush.Enqueue(RedisScript.Encode("delete", Key, command, Merge, null, () =>
            {
                if (previous != null) IdentityMap.SetReference(previous);
            }, type));
            return false;
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
