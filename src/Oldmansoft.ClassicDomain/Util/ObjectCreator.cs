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
                if (type.IsInterface)
                {
                    if (type.IsGenericList())
                    {
                        creator = new GenericListCreator(type);
                    }
                    else if (type.IsGenericDictionary())
                    {
                        creator = new GenericDictionaryCreator(type);
                    }
                    else
                    {
                        creator = EmptyCreator.Instance;
                    }
                }
                else if (type.IsAbstract)
                {
                    creator = EmptyCreator.Instance;
                }
                else
                {
                    var constructor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[0], null);
                    if (constructor == null)
                    {
                        creator = EmptyCreator.Instance;
                    }
                    else
                    {
                        creator = new NormalClassCreator(constructor);
                    }
                }
                Creators.TryAdd(type, creator);
            }
            return creator.CreateObject();
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
