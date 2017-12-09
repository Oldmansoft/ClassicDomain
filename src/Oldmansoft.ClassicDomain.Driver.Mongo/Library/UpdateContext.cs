using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oldmansoft.ClassicDomain.Util;
using MongoDB.Driver.Builders;
using MongoDB.Bson;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Library
{
    /// <summary>
    /// 更新内容上下文
    /// </summary>
    internal class UpdateContext
    {
        /// <summary>
        /// 获取上下文
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="compareSource"></param>
        /// <param name="compareTarget"></param>
        /// <returns></returns>
        public static UpdatedItem GetContext(object id, Type type, object compareSource, object compareTarget)
        {
            var result = new UpdatedItem(id.ToBsonValue());
            GetUpdateContext(type, result, string.Empty, compareSource, compareTarget);
            return result;
        }

        private static void GetUpdateContext(Type type, UpdatedItem result, string prefixName, object compareSource, object compareTarget)
        {
            foreach (var property in TypePublicInstanceStore.GetPropertys(type))
            {
                var propertyName = property.Name;
                if (propertyName.ToLower() == "id") propertyName = "_id";
                var name = string.Format("{0}{1}", prefixName, propertyName);
                var sourceValue = compareSource == null ? null : property.GetValue(compareSource);
                var targetValue = compareTarget == null ? null : property.GetValue(compareTarget);
                var propertyType = property.PropertyType;
                if (propertyType.IsArray || (propertyType.IsGenericType && propertyType.GetInterfaces().Contains(typeof(System.Collections.IEnumerable))))
                {
                    if (propertyType.GetInterfaces().Contains(typeof(System.Collections.IDictionary)))
                    {
                        DealDictionary(result, propertyType, name, sourceValue, targetValue);
                        continue;
                    }

                    DealList(result, propertyType, name, sourceValue, targetValue);
                    continue;
                }

                if (propertyType.IsNormalClass())
                {
                    DealNormalClass(result, propertyType, name, sourceValue, targetValue);
                    continue;
                }

                if (!sourceValue.IsEquals(targetValue))
                {
                    DealValue(result, name, targetValue);
                }
            }
        }

        private static void DealNormalClass(UpdatedItem result, Type type, string name, object sourceValue, object targetValue)
        {
            if (sourceValue == null && targetValue == null)
            {
                return;
            }
            if (sourceValue != null && targetValue != null)
            {
                GetUpdateContext(type, result, string.Format("{0}.", name), sourceValue, targetValue);
                return;
            }
            result.Add(new UpdateBuilder().SetObjectWrapped(name, targetValue));
        }

        private static void DealValue(UpdatedItem result, string name, object targetValue)
        {
            result.Add(Update.Set(name, targetValue.ToBsonValue()));
        }

        private static void DealList(UpdatedItem result, Type propertyType, string name, object sourceValue, object targetValue)
        {
            if (sourceValue == null && targetValue == null)
            {
                return;
            }
            if (sourceValue != null && targetValue != null)
            {
                DealListChange(result, propertyType, name, sourceValue, targetValue);
                return;
            }
            result.Add(new UpdateBuilder().SetObjectWrapped(name, targetValue));
        }

        private static void DealListChange(UpdatedItem result, Type propertyType, string name, object sourceValue, object targetValue)
        {
            var itemType = propertyType.IsArray ? propertyType.GetMethod("Set").GetParameters()[1].ParameterType : propertyType.GetGenericArguments()[0];
            var isNormalClass = itemType.IsNormalClass();
            var sourceList = sourceValue as System.Collections.IList;
            var targetList = targetValue as System.Collections.IList;

            if (IsByteArray(propertyType, itemType))
            {
                if (IsByteArrayContentDifferent(sourceList, targetList))
                {
                    result.Add(new UpdateBuilder().SetObjectWrapped(name, targetValue));
                }
                return;
            }

            for (var i = 0; i < targetList.Count; i++)
            {
                if (i >= sourceList.Count)
                {
                    result.AddOther(Update.Push(name, GetBsonValue(itemType, targetList[i], isNormalClass)));
                    continue;
                }
                if (isNormalClass)
                {
                    DealNormalClass(result, itemType, GetListKey(name, i), sourceList[i], targetList[i]);
                    continue;
                }
                if (!sourceList[i].IsEquals(targetList[i]))
                {
                    result.Add(Update.Set(GetListKey(name, i), GetBsonValue(itemType, targetList[i])));
                }
            }

            var isRemoveObject = false;
            for (var i = targetList.Count; i < sourceList.Count; i++)
            {
                isRemoveObject = true;
                result.AddOther(new UpdateBuilder().SetObjectWrapped(GetListKey(name, i), RemoveObject.Instance));
            }
            if (isRemoveObject) result.AddOther(new UpdateBuilder().PullObjectWrapped(name, RemoveObject.Instance));
        }

        private static bool IsByteArray(Type propertyType, Type itemType)
        {
            return propertyType.IsArray && itemType == typeof(byte);
        }

        private static bool IsByteArrayContentDifferent(System.Collections.IList sourceList, System.Collections.IList targetList)
        {
            if (targetList.Count != sourceList.Count) return true;
            for (var i = 0; i < targetList.Count; i++)
            {
                if ((byte)sourceList[i] != (byte)targetList[i])
                {
                    return true;
                }
            }
            return false;
        }

        private static BsonValue GetBsonValue(Type type, object value, bool isNormalClass = false)
        {
            if (isNormalClass)
            {
                return BsonDocumentWrapper.Create(type, value);
            }
            return value.ToBsonValue();
        }

        private static void DealDictionary(UpdatedItem result, Type propertyType, string name, object sourceValue, object targetValue)
        {
            if (sourceValue == null && targetValue == null)
            {
                return;
            }
            if (sourceValue != null && targetValue != null)
            {
                DealDictionaryChange(result, propertyType, name, sourceValue, targetValue);
                return;
            }
            result.Add(new UpdateBuilder().SetObjectWrapped(name, targetValue));
        }

        private static void DealDictionaryChange(UpdatedItem result, Type propertyType, string name, object sourceValue, object targetValue)
        {
            var keyType = propertyType.GetGenericArguments()[0];
            var valueType = propertyType.GetGenericArguments()[1];
            var isNormalClass = valueType.IsNormalClass();
            var sourceDictionary = sourceValue as System.Collections.IDictionary;
            var targetDictionary = targetValue as System.Collections.IDictionary;
            foreach (var key in targetDictionary.Keys)
            {   
                if (!sourceDictionary.Contains(key))
                {
                    result.Add(Update.Set(GetHashKey(name, key.ToString()), GetBsonValue(valueType, targetDictionary[key], isNormalClass)));
                    continue;
                }
                if (isNormalClass)
                {
                    DealNormalClass(result, valueType, GetHashKey(name, key.ToString()), sourceDictionary[key], targetDictionary[key]);
                    continue;
                }
                if (!sourceDictionary[key].IsEquals(targetDictionary[key]))
                {
                    result.Add(Update.Set(GetHashKey(name, key.ToString()), GetBsonValue(valueType, targetDictionary[key], isNormalClass)));
                }
            }
            foreach (var key in sourceDictionary.Keys)
            {
                if (!targetDictionary.Contains(key))
                {
                    result.AddOther(Update.Unset(GetHashKey(name, key.ToString())));
                }
            }
        }

        private static string GetHashKey(string name, string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentException(string.Format("属性 {0} 的键不允许为空", name));
            if (key.IndexOf('.') > -1) throw new ArgumentException(string.Format("属性 {0} 的键不允许有字符“.”", name));
            if (key.IndexOf('$') > -1) throw new ArgumentException(string.Format("属性 {0} 的键不允许有字符“$”", name));
            return string.Format("{0}.{1}", name, key);
        }

        private static string GetListKey(string name, int index)
        {
            return string.Format("{0}.{1}", name, index);
        }
    }
}
