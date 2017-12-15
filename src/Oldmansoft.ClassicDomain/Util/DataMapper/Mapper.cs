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
        private static SafeAddDictionary<MapType, IMap[]> Maps;

        static Mapper()
        {
            Maps = new SafeAddDictionary<MapType, IMap[]>(CreateContent);
        }

        public static IMap[] GetMapper(Type sourceType, Type targetType)
        {
            var key = new MapType(sourceType, targetType);
            return Maps.GetOrAdd(key);
        }

        private static IMap[] CreateContent(MapType type)
        {
            var result = new List<IMap>();

            if (type.Source.IsArray && type.Target.IsArray)
            {
                result.Add(new MapArray().Init(type.Source, type.Target));
                return result.ToArray();
            }
            else if (new Type[] { type.Source, type.Target }.IsGenericList())
            {
                result.Add(new MapList().Init(type.Source, type.Target));
                return result.ToArray();
            }
            else if (new Type[] { type.Source, type.Target }.IsGenericDictionary())
            {
                if (type.Source.GetGenericArguments()[0] != type.Target.GetGenericArguments()[0]) return result.ToArray();
                result.Add(new MapDictionary().Init(type.Source, type.Target));
                return result.ToArray();
            }
            EachPropertys(type.Source, type.Target, result);
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
            var normals = new MapStructPropertyCreator(sourceType, targetType);
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
                    result.Add(new MapArrayProperty().Init(sourceType, targetType, sourcePropertyInfo, targetPropertyInfo));
                    continue;
                }

                if (new Type[] { sourcePropertyType, targetPropertyType }.IsGenericList())
                {
                    var sourceItemType = sourcePropertyType.GetGenericArguments()[0];
                    var targetItemType = targetPropertyType.GetGenericArguments()[0];
                    result.Add(new MapListProperty().Init(sourceType, targetType, sourcePropertyInfo, targetPropertyInfo));
                    continue;
                }

                if (new Type[] { sourcePropertyType, targetPropertyType }.IsGenericDictionary())
                {
                    if (sourcePropertyType.GetGenericArguments()[0] != targetPropertyType.GetGenericArguments()[0]) continue;
                    result.Add(new MapDictionaryProperty().Init(sourceType, targetType, sourcePropertyInfo, targetPropertyInfo));
                    continue;
                }

                if (sourcePropertyType.IsEnum && targetPropertyType.IsEnum)
                {
                    result.Add(new MapEnumProperty().Init(sourceType, targetType, sourcePropertyInfo, targetPropertyInfo));
                    continue;
                }

                if (sourcePropertyType.IsNullableEnum() && targetPropertyType.IsNullableEnum())
                {
                    result.Add(new MapNullableEnumProperty().Init(sourceType, targetType, sourcePropertyInfo, targetPropertyInfo));
                    continue;
                }

                if (new Type[] { sourcePropertyType, targetPropertyType }.IsNormalClass())
                {
                    result.Add(new MapNormalClassProperty().Init(sourceType, targetType, sourcePropertyInfo, targetPropertyInfo));
                    continue;
                }

                if (sourcePropertyType == targetPropertyType)
                {
                    normals.Add(sourcePropertyInfo);
                }
            }
            if (normals.HasValue()) result.Add(normals.CreateMap());
        }
    }
}