using Oldmansoft.ClassicDomain.Driver.Redis.Library;
using Oldmansoft.ClassicDomain.Util;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

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
            command.Execute(Db, Merge);
            if (!Db.KeyDelete(MergeKey(command.Key))) return false;
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
            SetContext(domainType, result, new string[0]);
            return result;
        }

        private void SetContext(Type type, UpdatedCommand result, string[] prefixNames)
        {
            foreach (var property in TypePublicInstancePropertyInfoStore.GetPropertys(type))
            {
                var currentNames = prefixNames.AddToNew(property.Name);
                var propertyType = property.PropertyType;
                if (propertyType.IsArrayOrGenericList() || propertyType.IsGenericDictionary())
                {
                    result.KeyDelete.Add(currentNames.JoinDot());
                    continue;
                }

                if (propertyType.IsNormalClass())
                {
                    SetContext(propertyType, result, currentNames);
                    continue;
                }
            }
        }
    }
}
