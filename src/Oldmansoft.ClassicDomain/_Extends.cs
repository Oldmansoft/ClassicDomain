using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class Extends
    {
        /// <summary>
        /// 分页
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IPagingCondition<TSource> Paging<TSource>(this IQueryable<TSource> source)
            where TSource : class
        {
            return new Util.Paging.PagingCondition<TSource>(source);
        }


        /// <summary>
        /// 复制到
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static TTarget CopyTo<TSource, TTarget>(this TSource source, TTarget target)
        {
            if (source is Util.DataMapper) throw new ArgumentException("请不要直接使用 DataMapper.CopyTo(target) 方法", "source");
            return Util.DataMapper.Map(source, target);
        }

    }
}
