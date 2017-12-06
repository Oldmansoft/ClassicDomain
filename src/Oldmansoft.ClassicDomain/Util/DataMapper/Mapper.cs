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
        private static System.Collections.Concurrent.ConcurrentDictionary<long, IMapContent[]> Maps;

        static Mapper()
        {
            Maps = new System.Collections.Concurrent.ConcurrentDictionary<long, IMapContent[]>();
        }

        public static IMapContent[] GetMapper(Type sourceType, Type targetType)
        {
            var key = (long)sourceType.GetHashCode() * int.MaxValue + targetType.GetHashCode();
            IMapContent[] maps;
            if (Maps.TryGetValue(key, out maps)) return maps;

            var result = new List<IMapContent>();

            if (sourceType.IsArray && targetType.IsArray)
            {
                result.Add(new MapArray().Init(string.Empty, sourceType, targetType));
                return result.ToArray();
            }
            else if (new Type[] { sourceType, targetType }.IsGenericList())
            {
                result.Add(new MapList().Init(string.Empty, sourceType, targetType));
                return result.ToArray();
            }
            else if (new Type[] { sourceType, targetType }.IsGenericDictionary())
            {
                result.Add(new MapDictionary().Init(string.Empty, sourceType, targetType));
                return result.ToArray();
            }
            EachPropertys(sourceType, targetType, result);

            Maps.TryAdd(key, result.ToArray());
            return result.ToArray();
        }

        /// <summary>
        /// 查找属性
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="targetType"></param>
        /// <param name="result"></param>
        private static void EachPropertys(Type sourceType, Type targetType, List<IMapContent> result)
        {
            foreach (var sourcePropertyInfo in TypePublicInstanceStore.GetPropertys(sourceType))
            {
                if (!sourcePropertyInfo.CanRead) continue;

                var targetPropertyInfo = targetType.GetProperty(sourcePropertyInfo.Name, BindingFlags.Public | BindingFlags.Instance);
                if (targetPropertyInfo == null) continue;
                if (!targetPropertyInfo.CanWrite) continue;
                var sourcePropertyType = sourcePropertyInfo.PropertyType;
                var targetPropertyType = targetPropertyInfo.PropertyType;

                if (sourcePropertyType.IsArray && targetPropertyType.IsArray)
                {
                    result.Add(new MapArrayProperty().Init(sourcePropertyInfo.Name, sourcePropertyType, targetPropertyType, sourcePropertyInfo, targetPropertyInfo));
                    continue;
                }

                if (new Type[] { sourcePropertyType, targetPropertyType }.IsGenericList())
                {
                    var sourceItemType = sourcePropertyType.GetGenericArguments()[0];
                    var targetItemType = targetPropertyType.GetGenericArguments()[0];
                    result.Add(new MapListProperty().Init(sourcePropertyInfo.Name, sourcePropertyType, targetPropertyType, sourcePropertyInfo, targetPropertyInfo));
                    continue;
                }

                if (new Type[] { sourcePropertyType, targetPropertyType }.IsGenericDictionary())
                {
                    var sourceKeyType = sourcePropertyType.GetGenericArguments()[0];
                    var sourceValueType = sourcePropertyType.GetGenericArguments()[1];
                    var targetKeyType = targetPropertyType.GetGenericArguments()[0];
                    var targetValueType = targetPropertyType.GetGenericArguments()[1];
                    if (sourceKeyType != targetKeyType) continue;

                    result.Add(new MapDictionaryProperty().Init(sourcePropertyInfo.Name, sourcePropertyType, targetPropertyType, sourcePropertyInfo, targetPropertyInfo));
                    continue;
                }

                if (sourcePropertyType.IsEnum && targetPropertyType.IsEnum)
                {
                    result.Add(new MapEnumProperty().Init(sourcePropertyInfo.Name, sourcePropertyType, targetPropertyType, sourcePropertyInfo, targetPropertyInfo));
                    continue;
                }

                if (sourcePropertyType.IsNullableEnum() && targetPropertyType.IsNullableEnum())
                {
                    result.Add(new MapNullableEnumProperty().Init(sourcePropertyInfo.Name, sourcePropertyType, targetPropertyType, sourcePropertyInfo, targetPropertyInfo));
                    continue;
                }

                if (new Type[] { sourcePropertyType, targetPropertyType }.IsNormalClass())
                {
                    result.Add(new MapNormalClassProperty().Init(sourcePropertyInfo.Name, sourcePropertyType, targetPropertyType, sourcePropertyInfo, targetPropertyInfo));
                    continue;
                }

                if (sourcePropertyType != targetPropertyType) continue;
                result.Add(new MapStructProperty().Init(sourcePropertyInfo.Name, sourcePropertyType, targetPropertyType, sourcePropertyInfo, targetPropertyInfo));
            }
        }
    }
}
