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
            SetContext(domainType, result, string.Empty);
            return result;
        }

        private static void SetContext(Type type, UpdatedCommand result, string prefixName)
        {
            foreach(var property in TypePublicInstanceStore.GetPropertys(type))
            {
                var name = string.Format("{0}{1}", prefixName, property.Name);
                var propertyType = property.PropertyType;
                if (propertyType.IsArrayOrGenericList())
                {
                    result.KeyDelete.Add(name);
                    continue;
                }

                if (propertyType.IsNormalClass())
                {
                    SetContext(propertyType, result, string.Format("{0}", name));
                    continue;
                }
            }
        }
    }
}
