using System;
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
            if (!source.GetInterfaces().Contains(typeof(System.Collections.IEnumerable))) return false;
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
        /// 分页
        /// </summary>
        /// <typeparam name="TDomain">领域</typeparam>
        /// <param name="source">源</param>
        /// <param name="index">页码</param>
        /// <param name="size">页大小</param>
        /// <param name="sort">排序</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public static IPageResult<TDomain> Page<TDomain>(this IQuery<TDomain> source,
            int index,
            int size,
            Func<IQueryable<TDomain>, IQueryable<TDomain>> sort,
            params System.Linq.Expressions.Expression<Func<TDomain, bool>>[] condition
        ) where TDomain : class
        {
            if (source == null) throw new ArgumentNullException("source");
            if (sort == null) throw new ArgumentNullException("sort");

            var query = source.Query();
            if (condition != null)
            {
                for (int i = 0; i < condition.Length; i++)
                {
                    if (condition[i] == null) continue;
                    query = query.Where(condition[i]);
                }
            }

            var result = new PageResult<TDomain>();
            result.TotalCount = query.Count();
            query = sort(query);
            result.List = query.Skip(size * (index - 1)).Take(size).ToList();
            return result;
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
    }
}
