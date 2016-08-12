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
    /// 类型公共实例存储器
    /// </summary>
    public static class TypePublicInstanceStore
    {
        private static ConcurrentDictionary<Type, PropertyInfo[]> Propertys { get; set; }

        static TypePublicInstanceStore()
        {
            Propertys = new ConcurrentDictionary<Type, PropertyInfo[]>();
        }

        private static PropertyInfo[] Init(Type type)
        {
            var list = new List<PropertyInfo>();
            foreach (var item in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!item.CanRead) continue;
                if (!item.CanWrite) continue;
                list.Add(item);
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
        public static PropertyInfo[] GetPropertys(Type type)
        {
            PropertyInfo[] result;
            if (Propertys.TryGetValue(type, out result))
            {
                return result;
            }
            return Init(type);
        }

        /// <summary>
        /// 获取属性列表
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static PropertyInfo[] GetPropertys<TEntity>()
        {
            return GetPropertys(typeof(TEntity));
        }
    }
}
