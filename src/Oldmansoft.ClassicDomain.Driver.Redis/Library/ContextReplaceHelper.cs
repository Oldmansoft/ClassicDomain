using Oldmansoft.ClassicDomain.Util;
using System;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Library
{
    class ContextReplaceHelper
    {
        public static UpdatedCommand<TKey> GetContext<TKey>(TKey key, Type domainType, object compareSource, object compareTarget)
        {
            var result = new UpdatedCommand<TKey>(key, domainType);
            if (!RedisAccessCompiler.TryDiff(domainType, compareSource, compareTarget, result))
            {
                SetContext(domainType, result, new string[0], compareSource, compareTarget);
            }
            return result;
        }

        static void SetContext(Type type, UpdatedCommand result, string[] prefixNames, object compareSource, object compareTarget)
        {
            foreach (var property in Util.TypePublicInstancePropertyInfoStore.GetValues(type))
            {
                var currentNames = prefixNames.AddToNew(property.Name);
                var sourceValue = compareSource == null ? null : property.Get(compareSource);
                var targetValue = compareTarget == null ? null : property.Get(compareTarget);
                var propertyType = property.Type;
                var path = currentNames.JoinDot();
                if (TypeKind.IsByteArray(propertyType))
                {
                    RedisAccess.DiffBytes(result, path, sourceValue, targetValue);
                    continue;
                }
                if (TypeKind.IsList(propertyType))
                {
                    RedisAccess.DiffList(result, propertyType, path, sourceValue, targetValue);
                    continue;
                }
                if (TypeKind.IsDictionary(propertyType))
                {
                    RedisAccess.DiffDictionary(result, propertyType, path, sourceValue, targetValue);
                    continue;
                }
                if (TypeKind.IsNormalClass(propertyType))
                {
                    RedisAccess.DiffClassMarker(result, path, sourceValue, targetValue, propertyType.FullName);
                    SetContext(propertyType, result, currentNames, sourceValue, targetValue);
                    continue;
                }
                RedisAccess.DiffScalar(result, path, sourceValue, targetValue);
            }
        }
    }
}
