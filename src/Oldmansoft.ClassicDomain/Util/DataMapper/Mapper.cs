using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    class Mapper
    {
        private static System.Collections.Concurrent.ConcurrentDictionary<long, IMap[]> Maps;

        static Mapper()
        {
            Maps = new System.Collections.Concurrent.ConcurrentDictionary<long, IMap[]>();
        }

        public static IMap[] GetMapper(Type sourceType, Type targetType)
        {
            var key = (long)sourceType.GetHashCode() * int.MaxValue + targetType.GetHashCode();
            IMap[] maps;
            if (Maps.TryGetValue(key, out maps)) return maps;

            var result = new List<IMap>();
            var types = new Type[] { sourceType, targetType };

            if (types.IsArray())
            {
                result.Add(new MapArray().Init(sourceType, targetType));
            }
            else if (types.IsGenericListOrIGenericList())
            {
                result.Add(new MapList().Init(sourceType, targetType));
            }
            else if (types.IsGenericDictionaryOrIGenericDictionary())
            {
                if (sourceType.GetGenericArguments()[0] == targetType.GetGenericArguments()[0])
                {
                    result.Add(new MapDictionary().Init(sourceType, targetType));
                }
            }
            else
            {
                EachPropertys(sourceType, targetType, result);
            }

            Maps.TryAdd(key, result.ToArray());
            return result.ToArray();
        }

        /// <summary>
        /// 查找属性
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="targetType"></param>
        /// <param name="result"></param>
        private static void EachPropertys(Type sourceType, Type targetType, List<IMap> result)
        {
            var normalMap = new MapStructPropertyCreator(sourceType, targetType);
            foreach (var sourcePropertyInfo in TypePublicInstancePropertyInfoStore.GetPropertys(sourceType))
            {
                if (!sourcePropertyInfo.CanRead) continue;

                var targetPropertyInfo = targetType.GetProperty(sourcePropertyInfo.Name, BindingFlags.Public | BindingFlags.Instance);
                if (targetPropertyInfo == null) continue;
                if (!targetPropertyInfo.CanWrite) continue;
                var sourcePropertyType = sourcePropertyInfo.PropertyType;
                var targetPropertyType = targetPropertyInfo.PropertyType;
                var propertyTypes = new Type[] { sourcePropertyType, targetPropertyType };

                if (propertyTypes.IsArray())
                {
                    result.Add(new MapArrayProperty().Init(sourceType, targetType, sourcePropertyInfo, targetPropertyInfo));
                    continue;
                }

                if (propertyTypes.IsGenericListOrIGenericList())
                {
                    var sourceItemType = sourcePropertyType.GetGenericArguments()[0];
                    var targetItemType = targetPropertyType.GetGenericArguments()[0];
                    result.Add(new MapListProperty().Init(sourceType, targetType, sourcePropertyInfo, targetPropertyInfo));
                    continue;
                }

                if (propertyTypes.IsGenericDictionaryOrIGenericDictionary())
                {
                    if (sourcePropertyType.GetGenericArguments()[0] != targetPropertyType.GetGenericArguments()[0]) continue;
                    result.Add(new MapDictionaryProperty().Init(sourceType, targetType, sourcePropertyInfo, targetPropertyInfo));
                    continue;
                }

                if (propertyTypes.IsEnum())
                {
                    result.Add(new MapEnumProperty().Init(sourceType, targetType, sourcePropertyInfo, targetPropertyInfo));
                    continue;
                }

                if (propertyTypes.IsNullableEnum())
                {
                    result.Add(new MapNullableEnumProperty().Init(sourceType, targetType, sourcePropertyInfo, targetPropertyInfo));
                    continue;
                }

                if (propertyTypes.IsNormalClass())
                {
                    result.Add(new MapNormalClassProperty().Init(sourceType, targetType, sourcePropertyInfo, targetPropertyInfo));
                    continue;
                }

                if (sourcePropertyType == targetPropertyType)
                {
                    normalMap.Add(sourcePropertyInfo);
                }
            }
            if (normalMap.HasValue()) result.Add(normalMap.CreateMap());
        }
    }
}