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
        private static System.Collections.Concurrent.ConcurrentDictionary<Type, ICreator> Creators;

        static ObjectCreator()
        {
            Creators = new System.Collections.Concurrent.ConcurrentDictionary<Type, ICreator>();
        }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="type">实例类型</param>
        /// <returns></returns>
        public static object CreateInstance(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            
            ICreator creator;
            if (!Creators.TryGetValue(type, out creator))
            {
                creator = CreateCreateor(type);
                Creators.TryAdd(type, creator);
            }
            return creator.CreateObject();
        }

        private static ICreator CreateCreateor(Type type)
        {
            if (type.IsInterface)
            {
                if (type.IsIGenericList() || type.IsIGenericCollection())
                {
                    return new GenericListCreator(type);
                }

                if (type.IsIGenericDictionary())
                {
                    return new GenericDictionaryCreator(type);
                }

                return EmptyCreator.Instance;
            }

            if (type.IsAbstract)
            {
                return EmptyCreator.Instance;
            }

            var constructor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[0], null);
            if (constructor == null)
            {
                return EmptyCreator.Instance;
            }
            return new NormalClassCreator(constructor);
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
