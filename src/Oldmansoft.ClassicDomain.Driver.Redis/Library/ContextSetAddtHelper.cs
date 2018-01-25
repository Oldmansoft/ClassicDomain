using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oldmansoft.ClassicDomain.Util;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Library
{
    class ContextSetAddtHelper
    {
        /// <summary>
        /// 获取添加项
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="key"></param>
        /// <param name="domainType"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static UpdatedCommand<TKey> GetContext<TKey>(TKey key, Type domainType, object context)
        {
            var result = new UpdatedCommand<TKey>(key);
            SetContext(domainType, context, result, new string[0]);
            return result;
        }

        private static void SetContext(Type type, object context, UpdatedCommand result, string[] prefixNames)
        {
            foreach(var property in TypePublicInstancePropertyInfoStore.GetValues(type))
            {
                var value = property.Get(context);
                if (value == null) continue;

                var propertyType = property.Type;
                var currentNames = prefixNames.AddToNew(property.Name);
                var name = currentNames.JoinDot();

                if (propertyType.IsArrayOrGenericList())
                {
                    result.HashSet.Add(name, propertyType.FullName);
                    result.ListRightPush.Add(name, propertyType.ConvertToList(value));
                    continue;
                }
                
                if (propertyType.IsGenericDictionary())
                {
                    result.HashSet.Add(name, propertyType.FullName);
                    result.HashSetList.Add(name, propertyType.ConvertToDictionary(value));
                    continue;
                }

                if (propertyType.IsNormalClass())
                {
                    result.HashSet.Add(name, propertyType.FullName);
                    SetContext(propertyType, value, result, currentNames);
                    continue;
                }

                result.HashSet.Add(name, value.ToString());
            }
        }
    }
}
