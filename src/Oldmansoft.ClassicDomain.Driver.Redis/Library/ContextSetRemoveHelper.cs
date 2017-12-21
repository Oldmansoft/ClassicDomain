using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oldmansoft.ClassicDomain.Util;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Library
{
    class ContextSetRemoveHelper
    {
        /// <summary>
        /// 获取移除项
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="key"></param>
        /// <param name="domainType"></param>
        /// <returns></returns>
        public static UpdatedCommand<TKey> GetContext<TKey>(TKey key, Type domainType)
        {
            var result = new UpdatedCommand<TKey>(key);
            SetContext(domainType, result, new string[0]);
            return result;
        }

        private static void SetContext(Type type, UpdatedCommand result, string[] prefixNames)
        {
            foreach (var property in TypePublicInstancePropertyInfoStore.GetPropertys(type))
            {
                var currentNames = prefixNames.AddToNew(property.Name);
                var propertyType = property.PropertyType;
                if (propertyType.IsArrayOrGenericList() || propertyType.IsGenericDictionary())
                {
                    result.KeyDelete.Add(currentNames.JoinDot());
                    continue;
                }

                if (propertyType.IsNormalClass())
                {
                    SetContext(propertyType, result, currentNames);
                    continue;
                }
            }
        }
    }
}
