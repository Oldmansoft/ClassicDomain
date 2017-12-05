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
        private static System.Collections.Concurrent.ConcurrentDictionary<Type, ConstructorInfo> Constructors;

        static ObjectCreator()
        {
            Constructors = new System.Collections.Concurrent.ConcurrentDictionary<Type, ConstructorInfo>();
        }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="type">实例类型</param>
        /// <returns></returns>
        public static object CreateInstance(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (type.IsAbstract) return null;

            ConstructorInfo constructor;
            if (!Constructors.TryGetValue(type, out constructor))
            {
                constructor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[0], null);
                Constructors.TryAdd(type, constructor);
            }
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
