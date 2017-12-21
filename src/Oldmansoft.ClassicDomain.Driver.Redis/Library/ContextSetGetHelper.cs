using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Oldmansoft.ClassicDomain.Util;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Library
{
    class ContextSetGetHelper
    {
        public static ReflectionItem GetReflection(Type type)
        {
            var result = new ReflectionItem();
            SetReflection(type, result, string.Empty);
            return result;
        }

        private static void SetReflection(Type type, ReflectionItem result, string prefixName)
        {
            foreach (var property in TypePublicInstanceStore.GetPropertys(type))
            {
                var name = string.Format("{0}{1}", prefixName, property.Name);
                var propertyType = property.PropertyType;
                if (propertyType.IsArrayOrGenericList())
                {
                    result.ListNames.Add(name);
                    continue;
                }

                if (propertyType.IsGenericDictionary())
                {
                    result.HashNames.Add(name);
                    continue;
                }

                if (propertyType.IsNormalClass())
                {
                    SetReflection(propertyType, result, string.Format("{0}.", name));
                    continue;
                }
            }
        }

        public static T GetContext<T>(DataGetMapping mapping) where T : class, new()
        {
            if (mapping == null || mapping.Fields.Count == 0) return default(T);
            var result = new T();
            SetContext(mapping, typeof(T), result, string.Empty);
            return result;
        }

        private static void SetContext<T>(DataGetMapping mapping, Type type, T instance, string prefixName)
        {
            foreach (var property in TypePublicInstanceStore.GetPropertys(type))
            {
                var name = string.Format("{0}{1}", prefixName, property.Name);
                var propertyType = property.PropertyType;
                if (propertyType.IsArray)
                {
                    SetArray(mapping, instance, property, name);
                    continue;
                }

                if (propertyType.IsDictionary())
                {
                    SetDictionary(mapping, instance, property, name);
                    continue;
                }

                if (propertyType.IsGenericList())
                {
                    SetList(mapping, instance, property, name);
                    continue;
                }

                if (!mapping.Fields.ContainsKey(name)) continue;
                if (propertyType.IsNormalClass())
                {
                    var obj = ObjectCreator.CreateInstance(propertyType);
                    SetContext(mapping, propertyType, obj, string.Format("{0}.", name));
                    property.SetValue(instance, obj);
                    continue;
                }

                var value = mapping.Fields[name];
                property.SetValue(instance, propertyType.FromString(value));
            }
        }

        private static void SetArray(DataGetMapping mapping, object instance, PropertyInfo property, string name)
        {
            if (!mapping.Lists.ContainsKey(name)) return;
            property.SetValue(instance, mapping.Lists[name].GetArrayFromString(property.PropertyType));
        }

        private static void SetDictionary(DataGetMapping mapping, object instance, PropertyInfo property, string name)
        {
            if (!mapping.Hashs.ContainsKey(name)) return;

            var propertyType = property.PropertyType;
            var keyType = propertyType.GetGenericArguments()[0];
            var valueType = propertyType.GetGenericArguments()[1];
            var isKeyNormalClass = keyType.IsNormalClass();
            var isValueNormalClass = valueType.IsNormalClass();
            var dictionaryType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
            if (propertyType == dictionaryType)
            {
                var dictionary = ObjectCreator.CreateInstance(dictionaryType) as System.Collections.IDictionary;
                foreach (var item in mapping.Hashs[name])
                {
                    dictionary.Add(item.Key.GetValueFromString(keyType, isKeyNormalClass), item.Value.GetValueFromString(valueType, isValueNormalClass));
                }
                property.SetValue(instance, dictionary);
                return;
            }

            throw new NotSupportedException(string.Format("不支持 {0} 类型序列化", propertyType.FullName));
        }

        private static void SetList(DataGetMapping mapping, object instance, PropertyInfo property, string name)
        {
            if (!mapping.Lists.ContainsKey(name)) return;

            var propertyType = property.PropertyType;
            var itemType = propertyType.GetEnumerableItemType();
            var listType = typeof(List<>).MakeGenericType(itemType);
            if (propertyType == listType)
            {
                property.SetValue(instance, mapping.Lists[name].GetListFromString(listType, itemType));
                return;
            }

            var queueType = typeof(Queue<>).MakeGenericType(itemType);
            if (propertyType == queueType)
            {
                property.SetValue(instance, Activator.CreateInstance(queueType, mapping.Lists[name].GetListFromString(listType, itemType)));
                return;
            }

            var stackType = typeof(Stack<>).MakeGenericType(itemType);
            if (propertyType == stackType)
            {
                property.SetValue(instance, Activator.CreateInstance(stackType, mapping.Lists[name].GetListFromString(listType, itemType)));
                return;
            }

            throw new NotSupportedException(string.Format("不支持 {0} 类型序列化", propertyType.FullName));
        }
    }
}
