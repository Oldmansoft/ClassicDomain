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
    public class TypePublicInstancePropertyValueStore
    {
        private static ConcurrentDictionary<Type, IValue[]> Propertys { get; set; }

        static TypePublicInstancePropertyValueStore()
        {
            Propertys = new ConcurrentDictionary<Type, IValue[]>();
        }

        private static IValue[] Init(Type type)
        {
            var list = new List<IValue>();
            foreach (var item in TypePublicInstancePropertyInfoStore.GetPropertys(type))
            {
                if (!item.CanRead) continue;
                if (!item.CanWrite) continue;
                list.Add((IValue)Activator.CreateInstance(typeof(PropertyWrapper<,>).MakeGenericType(type, item.PropertyType), item));
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
        public static IValue[] GetPropertys(Type type)
        {
            IValue[] result;
            if (Propertys.TryGetValue(type, out result))
            {
                return result;
            }
            return Init(type);
        }
    }
}
