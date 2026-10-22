using Oldmansoft.ClassicDomain.Driver.Redis.Library;
using Oldmansoft.ClassicDomain.Util;
using System;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Core.Commands
{
    class AddCommand<TDomain, TKey> : ICommand
    {
        private readonly IMergeKey<TKey> Merge;

        private readonly IdentityMap<TDomain> IdentityMap;

        private readonly IRedisFlush Flush;

        private readonly TKey Key;

        private readonly TDomain Domain;

        public AddCommand(IMergeKey<TKey> merge, IdentityMap<TDomain> identityMap, IRedisFlush flush, TKey key, TDomain domain)
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
            var command = GetContext(Key, type, Domain);
            var previous = IdentityMap.Get(Key);
            IdentityMap.SetReference(Domain);
            Flush.Enqueue(RedisScript.Encode("notexists", Key, command, Merge, type.FullName, () => Restore(previous), type));
            return false;
        }

        void Restore(TDomain previous)
        {
            if (previous == null) IdentityMap.Remove(Key);
            else IdentityMap.SetReference(previous);
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
            if (RedisAccessCompiler.TryWrite(domainType, context, result)) return result;
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
