using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    /// <summary>
    /// 对象创建器
    /// </summary>
    public static class ObjectCreator
    {
        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="type">实例类型</param>
        /// <returns></returns>
        public static object CreateInstance(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            var constructor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[0], null);
            if (constructor == null) return null;
            return constructor.Invoke(new object[0]);
        }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <returns></returns>
        public static T CreateInstance<T>()
        {
            return (T)CreateInstance(typeof(T));
        }
    }
}
