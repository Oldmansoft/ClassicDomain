using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oldmansoft.ClassicDomain.Util;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Library
{
    class ContextSetReplaceHelper
    {
        public static UpdatedItem<TKey> GetContext<TKey>(TKey key, Type domainType, object compareSource, object compareTarget)
        {
            var result = new UpdatedItem<TKey>(key);
            SetContext(domainType, result, string.Empty, compareSource, compareTarget);
            return result;
        }

        private static void SetContext(Type type, UpdatedItem result, string prefixName, object compareSource, object compareTarget)
        {
            foreach (var property in TypePublicInstanceStore.GetPropertys(type))
            {
                var name = string.Format("{0}{1}", prefixName, property.Name);
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

        private static void DealNormalClass(UpdatedItem result, Type propertyType, string name, object sourceValue, object targetValue)
        {
            if (sourceValue != null && targetValue == null)
            {
                result.RemoveEntryFromHash.Add(name);
            }
            else if (sourceValue == null && targetValue != null)
            {
                result.SetRangeInHash.Add(name, propertyType.FullName);
            }

            SetContext(propertyType, result, string.Format("{0}.", name), sourceValue, targetValue);
        }

        private static void DealValue(UpdatedItem result, string name, object targetValue)
        {
            if (targetValue == null)
            {
                result.RemoveEntryFromHash.Add(name);
            }
            else
            {
                result.SetRangeInHash.Add(name, targetValue.ToString());
            }
        }

        private static void DealList(UpdatedItem result, Type propertyType, string name, object sourceValue, object targetValue)
        {
            if (sourceValue != null && targetValue == null)
            {
                result.RemoveEntryFromHash.Add(name);
                result.Remove.Add(name);
            }
            else if (sourceValue == null && targetValue != null)
            {
                result.SetRangeInHash.Add(name, propertyType.FullName);
                result.Remove.Add(name);
                result.AddRangeToList.Add(name, propertyType.ConvertToList(targetValue));
            }
            else if (sourceValue != null && targetValue != null)
            {
                DealListChange(result, propertyType, name, sourceValue, targetValue);
            }
        }

        private static void DealListChange(UpdatedItem result, Type propertyType, string name, object sourceValue, object targetValue)
        {
            var diffrentList = new Dictionary<int, string>();
            var addList = new List<string>();
            var sourceList = propertyType.ConvertToList(sourceValue);
            var targetList = propertyType.ConvertToList(targetValue);
            for (var i = 0; i < targetList.Count; i++)
            {
                if (i < sourceList.Count)
                {
                    if (sourceList[i] != targetList[i])
                    {
                        diffrentList.Add(i, targetList[i]);
                    }
                }
                else
                {
                    addList.Add(targetList[i]);
                }
            }
            for (var i = targetList.Count; i < sourceList.Count; i++)
            {
                result.RemoveItemFromList.Add(name, sourceList[i]);
            }

            if (diffrentList.Count > 0) result.SetItemInList.Add(name, diffrentList);
            if (addList.Count > 0) result.AddRangeToList.Add(name, addList);
        }

        private static void DealDictionary(UpdatedItem result, Type propertyType, string name, object sourceValue, object targetValue)
        {
            if (sourceValue != null && targetValue == null)
            {
                result.RemoveEntryFromHash.Add(name);
                result.Remove.Add(name);
            }
            else if (sourceValue == null && targetValue != null)
            {
                result.SetRangeInHash.Add(name, propertyType.FullName);
                result.Remove.Add(name);
                result.SetRangeInHashes.Add(name, propertyType.ConvertToDictionary(targetValue));
            }
            else if (sourceValue != null && targetValue != null)
            {
                DealDictionaryChange(result, propertyType, name, sourceValue, targetValue);
            }
        }

        private static void DealDictionaryChange(UpdatedItem result, Type propertyType, string name, object sourceValue, object targetValue)
        {
            var diffrentDicionary = new Dictionary<string, string>();
            var removeDicionary = new List<string>();
            var sourceDictionary = propertyType.ConvertToDictionary(sourceValue);
            var targetDictionary = propertyType.ConvertToDictionary(targetValue);
            foreach (var item in targetDictionary)
            {
                if (sourceDictionary.ContainsKey(item.Key))
                {
                    if (sourceDictionary[item.Key] != item.Value)
                    {
                        diffrentDicionary.Add(item.Key, item.Value);
                    }
                }
                else
                {
                    diffrentDicionary.Add(item.Key, item.Value);
                }
            }
            foreach (var item in sourceDictionary)
            {
                if (!targetDictionary.ContainsKey(item.Key))
                {
                    removeDicionary.Add(item.Key);
                }
            }
            if (removeDicionary.Count > 0) result.RemoveEntryFromHashes.Add(name, removeDicionary);
            if (diffrentDicionary.Count > 0) result.SetRangeInHashes.Add(name, diffrentDicionary);
        }
    }
}
