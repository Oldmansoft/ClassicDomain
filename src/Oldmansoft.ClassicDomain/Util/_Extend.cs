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
    public static class Extend
    {

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
            return source.IsArray || (source.IsGenericType && source.GetInterfaces().Contains(typeof(IEnumerable)));
        }

        /// <summary>
        /// 是否为泛型列表
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsGenericList(this Type source)
        {
            return source.IsGenericType && source.GetInterfaces().Contains(typeof(IEnumerable));
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
        /// 复制到
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="isDeepCopy">是否深拷贝</param>
        /// <returns></returns>
        public static TTarget CopyTo<TSource, TTarget>(this TSource source, TTarget target, bool isDeepCopy = true)
        {
            if (source is DataMapper) throw new ArgumentException("请不要直接使用 DataMapper.CopyTo(target) 方法", "source");
            return new DataMapper(isDeepCopy).CopyTo(source, target);
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
    }
}
