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
    }
}
