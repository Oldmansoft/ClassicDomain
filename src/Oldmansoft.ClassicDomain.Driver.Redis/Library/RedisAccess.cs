using Oldmansoft.ClassicDomain.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Library
{
    static class RedisAccess
    {
        public static string Join(string prefix, string name)
        {
            if (string.IsNullOrEmpty(prefix)) return name;
            return prefix + "." + name;
        }

        public static void DiffScalar(UpdatedCommand result, string path, object sourceValue, object targetValue)
        {
            if (sourceValue.IsEquals(targetValue)) return;
            if (targetValue == null) result.HashDelete.Add(path);
            else result.HashSet.Add(path, Extends.FormatScalar(targetValue));
        }

        public static void DiffBytes(UpdatedCommand result, string path, object sourceValue, object targetValue)
        {
            var source = sourceValue as byte[];
            var target = targetValue as byte[];
            if (source == null && target == null) return;
            if (target == null)
            {
                result.HashDelete.Add(path);
                return;
            }
            if (source != null && source.SequenceEqual(target)) return;
            result.HashSetBinary[path] = target;
        }

        public static void DiffClassMarker(UpdatedCommand result, string path, object sourceValue, object targetValue, string typeName)
        {
            if (sourceValue != null && targetValue == null) result.HashDelete.Add(path);
            else if (sourceValue == null && targetValue != null) result.HashSet.Add(path, typeName);
        }

        public static void DiffList(UpdatedCommand result, string path, object sourceValue, object targetValue, RuntimeTypeHandle typeHandle)
        {
            DiffList(result, Type.GetTypeFromHandle(typeHandle), path, sourceValue, targetValue);
        }

        public static void DiffList(UpdatedCommand result, Type propertyType, string path, object sourceValue, object targetValue)
        {
            if (sourceValue != null && targetValue == null)
            {
                result.HashDelete.Add(path);
                result.KeyDelete.Add(path);
            }
            else if (sourceValue == null && targetValue != null)
            {
                result.HashSet.Add(path, propertyType.FullName);
                result.KeyDelete.Add(path);
                result.ListRightPush.Add(path, propertyType.ConvertToList(targetValue));
            }
            else if (sourceValue != null && targetValue != null)
            {
                DiffListChange(result, propertyType, path, sourceValue, targetValue);
            }
        }

        static void DiffListChange(UpdatedCommand result, Type propertyType, string name, object sourceValue, object targetValue)
        {
            var diffrentList = new Dictionary<int, string>();
            var addList = new List<string>();
            var sourceList = propertyType.ConvertToList(sourceValue);
            var targetList = propertyType.ConvertToList(targetValue);
            for (var i = 0; i < targetList.Count; i++)
            {
                if (i < sourceList.Count)
                {
                    if (sourceList[i] != targetList[i]) diffrentList.Add(i, targetList[i]);
                }
                else
                {
                    addList.Add(targetList[i]);
                }
            }
            if (addList.Count > 0) result.ListRightPush.Add(name, addList);

            var hasRemove = false;
            for (var i = targetList.Count; i < sourceList.Count; i++)
            {
                hasRemove = true;
                diffrentList.Add(i, RemoveObject.Instance);
            }

            if (diffrentList.Count > 0) result.ListSetByIndex.Add(name, diffrentList);
            if (hasRemove) result.ListRemove.Add(name, RemoveObject.Instance);
        }

        public static void DiffDictionary(UpdatedCommand result, string path, object sourceValue, object targetValue, RuntimeTypeHandle typeHandle)
        {
            DiffDictionary(result, Type.GetTypeFromHandle(typeHandle), path, sourceValue, targetValue);
        }

        public static void DiffDictionary(UpdatedCommand result, Type propertyType, string path, object sourceValue, object targetValue)
        {
            if (sourceValue != null && targetValue == null)
            {
                result.HashDelete.Add(path);
                result.KeyDelete.Add(path);
            }
            else if (sourceValue == null && targetValue != null)
            {
                result.HashSet.Add(path, propertyType.FullName);
                result.KeyDelete.Add(path);
                result.HashSetList.Add(path, propertyType.ConvertToDictionary(targetValue));
            }
            else if (sourceValue != null && targetValue != null)
            {
                DiffDictionaryChange(result, propertyType, path, sourceValue, targetValue);
            }
        }

        static void DiffDictionaryChange(UpdatedCommand result, Type propertyType, string name, object sourceValue, object targetValue)
        {
            var diffrentDicionary = new Dictionary<string, string>();
            var removeDicionary = new List<string>();
            var sourceDictionary = propertyType.ConvertToDictionary(sourceValue);
            var targetDictionary = propertyType.ConvertToDictionary(targetValue);
            foreach (var item in targetDictionary)
            {
                if (sourceDictionary.ContainsKey(item.Key))
                {
                    if (sourceDictionary[item.Key] != item.Value) diffrentDicionary.Add(item.Key, item.Value);
                }
                else
                {
                    diffrentDicionary.Add(item.Key, item.Value);
                }
            }
            foreach (var item in sourceDictionary)
            {
                if (!targetDictionary.ContainsKey(item.Key)) removeDicionary.Add(item.Key);
            }
            if (removeDicionary.Count > 0) result.HashDeleteList.Add(name, removeDicionary);
            if (diffrentDicionary.Count > 0) result.HashSetList.Add(name, diffrentDicionary);
        }

        public static void WriteScalar(UpdatedCommand result, string path, object value)
        {
            if (value == null) return;
            result.HashSet.Add(path, Extends.FormatScalar(value));
        }

        public static void WriteBytes(UpdatedCommand result, string path, object value)
        {
            if (value == null) return;
            result.HashSetBinary[path] = (byte[])value;
        }

        public static void WriteClassMarker(UpdatedCommand result, string path, string typeName)
        {
            result.HashSet.Add(path, typeName);
        }

        public static void WriteList(UpdatedCommand result, string path, object value, RuntimeTypeHandle typeHandle)
        {
            if (value == null) return;
            var propertyType = Type.GetTypeFromHandle(typeHandle);
            result.HashSet.Add(path, propertyType.FullName);
            result.ListRightPush.Add(path, propertyType.ConvertToList(value));
        }

        public static void WriteDictionary(UpdatedCommand result, string path, object value, RuntimeTypeHandle typeHandle)
        {
            if (value == null) return;
            var propertyType = Type.GetTypeFromHandle(typeHandle);
            result.HashSet.Add(path, propertyType.FullName);
            result.HashSetList.Add(path, propertyType.ConvertToDictionary(value));
        }

        public static bool HasField(DataGetMapping mapping, string path)
        {
            return mapping.Fields.ContainsKey(path);
        }

        public static string Field(DataGetMapping mapping, string path)
        {
            return mapping.Fields[path];
        }

        public static object Parse(string value, RuntimeTypeHandle typeHandle)
        {
            return Type.GetTypeFromHandle(typeHandle).FromString(value);
        }

        public static object Create(RuntimeTypeHandle typeHandle)
        {
            return ObjectCreator.CreateInstance(Type.GetTypeFromHandle(typeHandle));
        }

        public static byte[] ReadBytes(DataGetMapping mapping, string path)
        {
            byte[] value;
            if (!mapping.Bytes.TryGetValue(path, out value)) return null;
            return value;
        }

        public static object ReadList(DataGetMapping mapping, string path, RuntimeTypeHandle typeHandle)
        {
            if (!mapping.Lists.ContainsKey(path)) return null;
            var propertyType = Type.GetTypeFromHandle(typeHandle);
            var values = mapping.Lists[path];
            if (propertyType.IsArray) return values.GetArrayFromString(propertyType);

            var itemType = propertyType.GetEnumerableItemType();
            var listType = typeof(List<>).MakeGenericType(itemType);
            if (propertyType == listType) return values.GetListFromString(listType, itemType);

            var queueType = typeof(Queue<>).MakeGenericType(itemType);
            if (propertyType == queueType) return Activator.CreateInstance(queueType, values.GetListFromString(listType, itemType));

            var stackType = typeof(Stack<>).MakeGenericType(itemType);
            if (propertyType == stackType) return Activator.CreateInstance(stackType, values.GetListFromString(listType, itemType));

            throw new NotSupportedException(string.Format("不支持 {0} 类型序列化", propertyType.FullName));
        }

        public static object ReadDictionary(DataGetMapping mapping, string path, RuntimeTypeHandle typeHandle)
        {
            if (!mapping.Hashs.ContainsKey(path)) return null;
            var propertyType = Type.GetTypeFromHandle(typeHandle);
            var keyType = propertyType.GetGenericArguments()[0];
            var valueType = propertyType.GetGenericArguments()[1];
            var dictionaryType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
            if (propertyType != dictionaryType) throw new NotSupportedException(string.Format("不支持 {0} 类型序列化", propertyType.FullName));

            var dictionary = ObjectCreator.CreateInstance(dictionaryType) as IDictionary;
            var isKeyNormalClass = keyType.IsNormalClass();
            var isValueNormalClass = valueType.IsNormalClass();
            foreach (var item in mapping.Hashs[path])
            {
                dictionary.Add(item.Key.GetValueFromString(keyType, isKeyNormalClass), item.Value.GetValueFromString(valueType, isValueNormalClass));
            }
            return dictionary;
        }
    }
}
