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
        public static UpdatedItem<TKey> GetContext<TKey>(TKey key, Type domainType, object context)
        {
            var result = new UpdatedItem<TKey>(key);
            SetContext(domainType, context, result, string.Empty);
            return result;
        }

        private static void SetContext(Type type, object context, UpdatedItem result, string prefixName)
        {
            foreach(var property in TypePublicInstanceStore.GetPropertys(type))
            {
                var value = property.GetValue(context);
                if (value == null) continue;

                var propertyType = property.PropertyType;
                var name = string.Format("{0}{1}", prefixName, property.Name);

                result.SetRangeInHash.Add(name, propertyType.FullName);
                if (propertyType.IsArrayOrGenericList())
                {
                    if (propertyType.IsDictionary())
                    {
                        result.SetRangeInHashes.Add(name, propertyType.ConvertToDictionary(value));
                        continue;
                    }

                    result.AddRangeToList.Add(name, propertyType.ConvertToList(value));
                    continue;
                }

                if (propertyType.IsNormalClass())
                {
                    result.SetRangeInHash.Add(name, propertyType.FullName);
                    SetContext(propertyType, value, result, string.Format("{0}.", name));
                    continue;
                }

                result.SetRangeInHash.Add(name, property.GetValue(context).ToString());
            }
        }
    }
}
