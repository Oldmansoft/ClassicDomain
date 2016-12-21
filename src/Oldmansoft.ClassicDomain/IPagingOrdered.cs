using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain
{
    /// <summary>
    /// 分页排序
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    public interface IPagingOrdered<TSource> : IPagingData<TSource>
    {
        /// <summary>
        /// 再正序
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        IPagingOrdered<TSource> ThenBy<TKey>(Expression<Func<TSource, TKey>> keySelector);

        /// <summary>
        /// 再倒序
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        IPagingOrdered<TSource> ThenByDescending<TKey>(Expression<Func<TSource, TKey>> keySelector);
    }
}
