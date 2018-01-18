using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class Extends
    {
        /// <summary>
        /// 在数组后面添加新的项，并创建出新的数组
        /// </summary>
        /// <param name="source"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static T[] AddToNew<T>(this T[] source, T item)
        {
            var result = new T[source.Length + 1];
            if (source.Length > 0) Array.Copy(source, result, source.Length);
            result[result.Length - 1] = item;
            return result;
        }

        /// <summary>
        /// 加入点到字符串之间
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string JoinDot(this string[] source)
        {
            return string.Join<string>(".", source);
        }

        /// <summary>
        /// 两个对象是否内容相同
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool IsEquals(this object source, object target)
        {
            if (source == null && target == null) return true;
            if (source == null) return false;
            if (target == null) return false;
            return source.Equals(target);
        }
        
        /// <summary>
        /// 是否为非字符串的类
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNormalClass(this Type source)
        {
            if (!source.IsClass || source == typeof(string))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 是否为非字符串的类
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNormalClass(this IEnumerable<Type> source)
        {
            foreach(var item in source)
            {
                if (!item.IsNormalClass())
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 是否为数组或泛型列表
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsArrayOrGenericList(this Type source)
        {
            return source.IsArray || (source.IsGenericType && source.GetInterfaces().Contains(typeof(IList)));
        }

        /// <summary>
        /// 是否为集合
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsDictionary(this Type source)
        {
            return source.GetInterfaces().Contains(typeof(IDictionary));
        }

        /// <summary>
        /// 是否为可空枚举
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNullableEnum(this Type source)
        {
            return source.IsGenericType
                && source.GetGenericTypeDefinition() == typeof(Nullable<>)
                && source.GenericTypeArguments[0].IsEnum;
        }

        /// <summary>
        /// 是否为泛型枚举
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsGenericEnumerable(this Type source)
        {
            if (!source.IsGenericType) return false;
            if (!source.GetInterfaces().Contains(typeof(IEnumerable))) return false;
            return true;
        }

        /// <summary>
        /// 是否为泛型字典
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsGenericDictionary(this Type source)
        {
            if (!source.IsGenericType) return false;
            var sourceGenericTypeDefinition = source.GetGenericTypeDefinition();
            if (sourceGenericTypeDefinition == typeof(Dictionary<,>)) return true;
            return false;
        }

        /// <summary>
        /// 是否为泛型字典接口
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsIGenericDictionary(this Type source)
        {
            if (!source.IsGenericType) return false;
            var sourceGenericTypeDefinition = source.GetGenericTypeDefinition();
            if (sourceGenericTypeDefinition == typeof(IDictionary<,>)) return true;
            return false;
        }

        /// <summary>
        /// 是否为泛型字典或其接口
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsGenericDictionaryOrIGenericDictionary(this Type source)
        {
            if (!source.IsGenericType) return false;
            var sourceGenericTypeDefinition = source.GetGenericTypeDefinition();
            if (sourceGenericTypeDefinition == typeof(Dictionary<,>)) return true;
            if (sourceGenericTypeDefinition == typeof(IDictionary<,>)) return true;
            return false;
        }

        /// <summary>
        /// 是否为泛型列表
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsGenericList(this Type source)
        {
            if (!source.IsGenericType) return false;
            var sourceGenericTypeDefinition = source.GetGenericTypeDefinition();
            if (sourceGenericTypeDefinition == typeof(List<>)) return true;
            return false;
        }

        /// <summary>
        /// 是否为泛型列表接口
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsIGenericList(this Type source)
        {
            if (!source.IsGenericType) return false;
            var sourceGenericTypeDefinition = source.GetGenericTypeDefinition();
            if (sourceGenericTypeDefinition == typeof(IList<>)) return true;
            return false;
        }

        /// <summary>
        /// 是否为泛型列表
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsGenericListOrIGenericList(this Type source)
        {
            if (!source.IsGenericType) return false;
            var sourceGenericTypeDefinition = source.GetGenericTypeDefinition();
            if (sourceGenericTypeDefinition == typeof(List<>)) return true;
            if (sourceGenericTypeDefinition == typeof(IList<>)) return true;
            return false;
        }

        /// <summary>
        /// 是否为泛型列表接口
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsIGenericCollection(this Type source)
        {
            if (!source.IsGenericType) return false;
            var sourceGenericTypeDefinition = source.GetGenericTypeDefinition();
            if (sourceGenericTypeDefinition == typeof(ICollection<>)) return true;
            return false;
        }

        /// <summary>
        /// 是否为泛型枚举
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsGenericEnumerable(this IEnumerable<Type> source)
        {
            foreach (var item in source)
            {
                if (!item.IsGenericEnumerable())
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 是否为数组
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsArray(this IEnumerable<Type> source)
        {
            foreach (var item in source)
            {
                if (!item.IsArray)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 是否为枚举
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsEnum(this IEnumerable<Type> source)
        {
            foreach (var item in source)
            {
                if (!item.IsEnum)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 是否为可空枚举
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNullableEnum(this IEnumerable<Type> source)
        {
            foreach(var item in source)
            {
                if (!item.IsNullableEnum())
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 是否为泛型字典
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsGenericDictionary(this IEnumerable<Type> source)
        {
            foreach (var item in source)
            {
                if (!item.IsGenericDictionary())
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 是否为泛型字典接口
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsIGenericDictionary(this IEnumerable<Type> source)
        {
            foreach (var item in source)
            {
                if (!item.IsIGenericDictionary())
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 是否为泛型字典或其接口
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsGenericDictionaryOrIGenericDictionary(this IEnumerable<Type> source)
        {
            foreach (var item in source)
            {
                if (!item.IsGenericDictionaryOrIGenericDictionary())
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 是否为泛型列表
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsGenericList(this IEnumerable<Type> source)
        {
            foreach (var item in source)
            {
                if (!item.IsGenericList())
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 是否为泛型列表接口
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsIGenericList(this IEnumerable<Type> source)
        {
            foreach (var item in source)
            {
                if (!item.IsIGenericList())
                {
                    return false;
                }
            }
            return true;
        }
        
        /// <summary>
        /// 是否为泛型列表或其接口
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsGenericListOrIGenericList(this IEnumerable<Type> source)
        {
            foreach (var item in source)
            {
                if (!item.IsGenericListOrIGenericList())
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 获取可枚举项的类型
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Type GetEnumerableItemType(this Type source)
        {
            if (source.IsArray)
            {
                return source.GetMethod("Set").GetParameters()[1].ParameterType;
            }
            if (source.IsGenericType)
            {
                return source.GetGenericArguments()[0];
            }
            throw new NotSupportedException(string.Format("不支持获取 {0} 此类型的子项类型。", source.FullName));
        }
        
        /// <summary>
        /// 转换字符串为值
        /// </summary>
        /// <param name="source"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static object FromString(this Type source, string context)
        {
            return TypeParse.Get(source)(context);
        }
        
        /// <summary>
        /// 创建属性的获值方法委托
        /// </summary>
        /// <typeparam name="TCaller"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Func<TCaller, TValue> CreateGetMethod<TCaller, TValue>(string name)
        {
            return (Func<TCaller, TValue>)Delegate.CreateDelegate(typeof(Func<TCaller, TValue>), typeof(TCaller).GetProperty(name).GetGetMethod(true));
        }

        /// <summary>
        /// 创建属性的赋值方法委托
        /// </summary>
        /// <typeparam name="TCaller"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Action<TCaller, TValue> CreateSetMethod<TCaller, TValue>(string name)
        {
            return (Action<TCaller, TValue>)Delegate.CreateDelegate(typeof(Action<TCaller, TValue>), typeof(TCaller).GetProperty(name).GetSetMethod(true));
        }
    }
}
