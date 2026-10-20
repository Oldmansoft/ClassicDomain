using Oldmansoft.ClassicDomain.Driver.Redis.Library;
using Oldmansoft.ClassicDomain.Util;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Core.Commands
{
    class AddCommand<TDomain, TKey> : ICommand
    {
        private readonly IDatabase Db;

        private readonly Func<TKey, string> MergeKey;

        private readonly IMergeKey<TKey> Merge;

        private readonly IdentityMap<TDomain> IdentityMap;

        private readonly TKey Key;

        private readonly TDomain Domain;

        public AddCommand(IDatabase db, Func<TKey, string> mergeKey, IMergeKey<TKey> merge, IdentityMap<TDomain> identityMap, TKey key, TDomain domain)
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
            var command = GetContext(Key, type, Domain);
            var transaction = Db.CreateTransaction();
            transaction.AddCondition(Condition.HashNotExists(MergeKey(command.Key), "this"));
            var tasks = new List<Task>();
            tasks.Add(transaction.HashSetAsync(MergeKey(command.Key), "this", type.FullName));
            command.Queue(transaction, Merge, tasks);
            try
            {
                if (!transaction.Execute()) return false;
            }
            catch (RedisServerException ex)
            {
                if (ex.Message == "ERR Operation against a key holding the wrong kind of value")
                {
                    throw new ClassicDomainException(type, "数据冲突：存在着相同记录的不同类型数据，可能是之前使用过快速模式保存过。");
                }
                throw;
            }
            command.Wait(tasks);
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

                if (TypeKind.IsByteArray(propertyType))
                {
                    result.HashSetBinary[name] = (byte[])value;
                    continue;
                }

                if (TypeKind.IsList(propertyType))
                {
                    result.HashSet.Add(name, propertyType.FullName);
                    result.ListRightPush.Add(name, propertyType.ConvertToList(value));
                    continue;
                }

                if (TypeKind.IsDictionary(propertyType))
                {
                    result.HashSet.Add(name, propertyType.FullName);
                    result.HashSetList.Add(name, propertyType.ConvertToDictionary(value));
                    continue;
                }

                if (TypeKind.IsNormalClass(propertyType))
                {
                    result.HashSet.Add(name, propertyType.FullName);
                    SetContext(propertyType, value, result, currentNames);
                    continue;
                }

                result.HashSet.Add(name, Extends.FormatScalar(value));
            }
        }
    }
}
