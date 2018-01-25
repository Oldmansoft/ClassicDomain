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
    /// 类型公共实例属性存储器
    /// </summary>
    public static class TypePublicInstancePropertyInfoStore
    {
        private static ConcurrentDictionary<Type, IDictionary<PropertyInfo, IValue>> Propertys { get; set; }

        static TypePublicInstancePropertyInfoStore()
        {
            Propertys = new ConcurrentDictionary<Type, IDictionary<PropertyInfo, IValue>>();
        }

        private static IDictionary<PropertyInfo, IValue> Init(Type type)
        {
            var result = new Dictionary<PropertyInfo, IValue>();
            foreach (var item in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!item.CanRead) continue;
                if (!item.CanWrite) continue;
                result.Add(item, (IValue)Activator.CreateInstance(typeof(PropertyWrapper<,>).MakeGenericType(type, item.PropertyType), item));
            }
            Propertys.TryAdd(type, result);
            return result;
        }

        /// <summary>
        /// 获取属性和值操作接口集
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static IDictionary<PropertyInfo, IValue> Get(Type type)
        {
            IDictionary<PropertyInfo, IValue> result;
            if (Propertys.TryGetValue(type, out result))
            {
                return result;
            }
            return Init(type);
        }

        /// <summary>
        /// 获取属性列表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetPropertys(Type type)
        {
            return Get(type).Keys;
        }

        /// <summary>
        /// 获取值操作接口列表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<IValue> GetValues(Type type)
        {
            return Get(type).Values;
        }

        /// <summary>
        /// 查找属性的值操作接口
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IValue GetValue<TEntity>(Func<PropertyInfo, bool> predicate)
        {
            var result = Get(typeof(TEntity));
            var key = result.Keys.First(predicate);
            if (key == null) return null;
            return result[key];
        }
    }
}
