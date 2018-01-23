using Oldmansoft.ClassicDomain.Driver.Redis.Library;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Core.Commands
{
    class SafeModeAddCommand<TDomain, TKey> : ICommand
    {
        private IDatabase Db;

        private Func<TKey, string> MergeKey;

        private IMergeKey<TKey> Merge;

        private IdentityMap<TDomain> IdentityMap;

        private TKey Key;

        private TDomain Domain;

        public SafeModeAddCommand(IDatabase db, Func<TKey, string> mergeKey, IMergeKey<TKey> merge, IdentityMap<TDomain> identityMap, TKey key, TDomain domain)
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
            var command = ContextSetAddtHelper.GetContext(Key, typeof(TDomain), Domain);
            try
            {
                if (!Db.HashSet(MergeKey(command.Key), "this", typeof(TDomain).FullName)) return false;
            }
            catch (RedisServerException ex)
            {
                if (ex.Message == "ERR Operation against a key holding the wrong kind of value")
                {
                    throw new ClassicDomainException("数据冲突：存在着相同记录的不同类型数据，可能是之前使用过快速模式保存过。");
                }
                else
                {
                    throw;
                }
            }
            command.Execute(Db, Merge);
            IdentityMap.Set(Domain);
            return true;
        }
    }
}
