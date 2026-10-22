using Oldmansoft.ClassicDomain.Driver.Redis.Library;
using System;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Core.Commands
{
    class ReplaceCommand<TDomain, TKey> : ICommand
    {
        private readonly IMergeKey<TKey> Merge;

        private readonly IdentityMap<TDomain> IdentityMap;

        private readonly IRedisFlush Flush;

        private readonly TKey Key;

        private readonly TDomain Domain;

        public ReplaceCommand(IMergeKey<TKey> merge, IdentityMap<TDomain> identityMap, IRedisFlush flush, TKey key, TDomain domain)
        {
            Merge = merge;
            IdentityMap = identityMap;
            Flush = flush;
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
            IdentityMap.SetReference(Domain);
            Flush.Enqueue(RedisScript.Encode("exists", Key, command, Merge, null, () => IdentityMap.SetReference(source), type));
            return false;
        }
    }
}
