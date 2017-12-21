using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    /// <summary>
    /// 类型公共实例属性取值存储器
    /// </summary>
    public class TypePublicInstancePropertyGetterStore
    {
        private static ConcurrentDictionary<Type, IGetterData[]> Propertys { get; set; }

        static TypePublicInstancePropertyGetterStore()
        {
            Propertys = new ConcurrentDictionary<Type, IGetterData[]>();
        }

        private static IGetterData[] Init(Type type)
        {
            var list = new List<IGetterData>();
            foreach (var item in TypePublicInstancePropertyInfoStore.GetPropertys(type))
            {
                if (!item.CanRead) continue;
                if (!item.CanWrite) continue;
                list.Add((IGetterData)Activator.CreateInstance(typeof(PropertyGetter<,>).MakeGenericType(type, item.PropertyType), item));
            }
            var result = list.ToArray();
            Propertys.TryAdd(type, result);
            return result;
        }

        /// <summary>
        /// 获取属性列表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IGetterData[] GetPropertys(Type type)
        {
            IGetterData[] result;
            if (Propertys.TryGetValue(type, out result))
            {
                return result;
            }
            return Init(type);
        }
    }
}
