using Oldmansoft.ClassicDomain.Driver.Redis.Library;
using Oldmansoft.ClassicDomain.Util;
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
            var command = GetContext(Key, typeof(TDomain), Domain);
            try
            {
                if (!Db.HashSet(MergeKey(command.Key), "this", typeof(TDomain).FullName)) return false;
            }
            catch (RedisServerException ex)
            {
                if (ex.Message == "ERR Operation against a key holding the wrong kind of value")
                {
                    throw new ClassicDomainException(Type, "数据冲突：存在着相同记录的不同类型数据，可能是之前使用过快速模式保存过。");
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

        /// <summary>
        /// 获取添加项
        /// </summary>
        /// <param name="key"></param>
        /// <param name="domainType"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private UpdatedCommand<TKey> GetContext(TKey key, Type domainType, object context)
        {
            var result = new UpdatedCommand<TKey>(key, domainType);
            SetContext(domainType, context, result, new string[0]);
            return result;
        }

        private void SetContext(Type type, object context, UpdatedCommand result, string[] prefixNames)
        {
            foreach (var property in TypePublicInstancePropertyInfoStore.GetValues(type))
            {
                var value = property.Get(context);
                if (value == null) continue;

                var propertyType = property.Type;
                var currentNames = prefixNames.AddToNew(property.Name);
                var name = currentNames.JoinDot();

                if (propertyType.IsArrayOrGenericList())
                {
                    result.HashSet.Add(name, propertyType.FullName);
                    result.ListRightPush.Add(name, propertyType.ConvertToList(value));
                    continue;
                }

                if (propertyType.IsGenericDictionary())
                {
                    result.HashSet.Add(name, propertyType.FullName);
                    result.HashSetList.Add(name, propertyType.ConvertToDictionary(value));
                    continue;
                }

                if (propertyType.IsNormalClass())
                {
                    result.HashSet.Add(name, propertyType.FullName);
                    SetContext(propertyType, value, result, currentNames);
                    continue;
                }

                result.HashSet.Add(name, value.ToString());
            }
        }
    }
}
