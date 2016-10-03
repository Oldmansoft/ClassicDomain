using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Oldmansoft.ClassicDomain.Driver.Redis.Library;
using Oldmansoft.ClassicDomain.Util;

namespace Oldmansoft.ClassicDomain.Driver.Redis
{
    static class Extends
    {
        private static string GetStringFromValue(object value, bool isNormalClass)
        {
            if (value == null) return null;

            if (isNormalClass)
            {
                return Serializer.Serialize(value);
            }

            return value.ToString();
        }

        public static List<string> ConvertToList(this Type source, object value)
        {
            var result = new List<string>();
            var isNormalClass = source.GetEnumerableItemType().IsNormalClass();
            foreach (var item in (IEnumerable)value)
            {
                result.Add(GetStringFromValue(item, isNormalClass));
            }
            return result;
        }

        public static Dictionary<string, string> ConvertToDictionary(this Type source, object value)
        {
            var result = new Dictionary<string, string>();
            var genericArguments = source.GetGenericArguments();
            var isItemKeyNormalClass = genericArguments[0].IsNormalClass();
            var isItemValueNormalClass = genericArguments[1].IsNormalClass();
            foreach (DictionaryEntry item in (IDictionary)value)
            {
                var itemKey = GetStringFromValue(item.Key, isItemKeyNormalClass);
                var itemValue = GetStringFromValue(item.Value, isItemValueNormalClass);
                if (itemValue == null) itemValue = string.Empty;
                result.Add(itemKey, itemValue);
            }
            return result;
        }

        public static List<string> ToStringList(this StackExchange.Redis.RedisValue[] source)
        {
            var result = new List<string>();
            foreach (var item in source)
            {
                result.Add(item);
            }
            return result;
        }

        public static StackExchange.Redis.RedisValue[] ToRedisValues(this List<string> source)
        {
            var result = new List<StackExchange.Redis.RedisValue>();
            foreach (var item in source)
            {
                result.Add(item);
            }
            return result.ToArray();
        }

        public static Dictionary<string, string> ToStringDictionary(this StackExchange.Redis.HashEntry[] source)
        {
            var result = new Dictionary<string, string>();
            foreach (var item in source)
            {
                result.Add(item.Name, item.Value);
            }
            return result;
        }

        public static StackExchange.Redis.HashEntry[] ToHashEntries(this Dictionary<string, string> source)
        {
            var result = new List<StackExchange.Redis.HashEntry>();
            foreach (var item in source)
            {
                result.Add(new StackExchange.Redis.HashEntry(item.Key, item.Value));
            }
            return result.ToArray();
        }

        public static object GetValueFromString(this string source, Type type, bool isNormalClass)
        {
            if (source == null) return null;

            if (isNormalClass)
            {
                return Serializer.Deserialize(source, type);
            }

            return type.FromString(source);
        }

        public static Array GetArrayFromString(this List<string> source, Type type)
        {
            var array = type.InvokeMember("Set", BindingFlags.CreateInstance, null, null, new object[] { source.Count }) as Array;
            var arrayItemType = type.GetMethod("Set").GetParameters()[1].ParameterType;
            var isArrayItemNormalClass = arrayItemType.IsNormalClass();
            for (var i = 0; i < source.Count; i++)
            {
                if (source[i] == null) continue;
                array.SetValue(source[i].GetValueFromString(arrayItemType, isArrayItemNormalClass), i);
            }
            return array;
        }
        
        public static IList GetListFromString(this List<string> source, Type listType, Type itemType)
        {
            var list = Activator.CreateInstance(listType) as IList;
            var isItemNormalClass = itemType.IsNormalClass();
            foreach (var item in source)
            {
                list.Add(item.GetValueFromString(itemType, isItemNormalClass));
            }
            return list;
        }
    }
}