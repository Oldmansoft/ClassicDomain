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
        public static UpdatedCommand<TKey> GetContext<TKey>(TKey key, Type domainType, object compareSource, object compareTarget)
        {
            var result = new UpdatedCommand<TKey>(key);
            SetContext(domainType, result, new string[0], compareSource, compareTarget);
            return result;
        }

        private static void SetContext(Type type, UpdatedCommand result, string[] prefixNames, object compareSource, object compareTarget)
        {
            foreach (var property in TypePublicInstancePropertyInfoStore.GetValues(type))
            {
                var currentNames = prefixNames.AddToNew(property.Name);
                var sourceValue = compareSource == null ? null : property.Get(compareSource);
                var targetValue = compareTarget == null ? null : property.Get(compareTarget);
                var propertyType = property.Type;
                if (propertyType.IsArrayOrGenericList())
                {
                    DealList(result, propertyType, currentNames.JoinDot(), sourceValue, targetValue);
                    continue;
                }

                if (propertyType.IsGenericDictionary())
                {
                    DealDictionary(result, propertyType, currentNames.JoinDot(), sourceValue, targetValue);
                    continue;
                }

                if (propertyType.IsNormalClass())
                {
                    DealNormalClass(result, propertyType, currentNames, sourceValue, targetValue);
                    continue;
                }

                if (!sourceValue.IsEquals(targetValue))
                {
                    DealValue(result, currentNames.JoinDot(), targetValue);
                }
            }
        }

        private static void DealNormalClass(UpdatedCommand result, Type propertyType, string[] prefixNames, object sourceValue, object targetValue)
        {
            if (sourceValue != null && targetValue == null)
            {
                result.HashDelete.Add(prefixNames.JoinDot());
            }
            else if (sourceValue == null && targetValue != null)
            {
                result.HashSet.Add(prefixNames.JoinDot(), propertyType.FullName);
            }

            SetContext(propertyType, result, prefixNames, sourceValue, targetValue);
        }

        private static void DealValue(UpdatedCommand result, string name, object targetValue)
        {
            if (targetValue == null)
            {
                result.HashDelete.Add(name);
            }
            else
            {
                result.HashSet.Add(name, targetValue.ToString());
            }
        }

        private static void DealList(UpdatedCommand result, Type propertyType, string name, object sourceValue, object targetValue)
        {
            if (sourceValue != null && targetValue == null)
            {
                result.HashDelete.Add(name);
                result.KeyDelete.Add(name);
            }
            else if (sourceValue == null && targetValue != null)
            {
                result.HashSet.Add(name, propertyType.FullName);
                result.KeyDelete.Add(name);
                result.ListRightPush.Add(name, propertyType.ConvertToList(targetValue));
            }
            else if (sourceValue != null && targetValue != null)
            {
                DealListChange(result, propertyType, name, sourceValue, targetValue);
            }
        }

        private static void DealListChange(UpdatedCommand result, Type propertyType, string name, object sourceValue, object targetValue)
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

        private static void DealDictionary(UpdatedCommand result, Type propertyType, string name, object sourceValue, object targetValue)
        {
            if (sourceValue != null && targetValue == null)
            {
                result.HashDelete.Add(name);
                result.KeyDelete.Add(name);
            }
            else if (sourceValue == null && targetValue != null)
            {
                result.HashSet.Add(name, propertyType.FullName);
                result.KeyDelete.Add(name);
                result.HashSetList.Add(name, propertyType.ConvertToDictionary(targetValue));
            }
            else if (sourceValue != null && targetValue != null)
            {
                DealDictionaryChange(result, propertyType, name, sourceValue, targetValue);
            }
        }

        private static void DealDictionaryChange(UpdatedCommand result, Type propertyType, string name, object sourceValue, object targetValue)
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
            if (removeDicionary.Count > 0) result.HashDeleteList.Add(name, removeDicionary);
            if (diffrentDicionary.Count > 0) result.HashSetList.Add(name, diffrentDicionary);
        }
    }
}
