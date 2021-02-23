using System;
using System.Linq.Expressions;

namespace Oldmansoft.ClassicDomain
{
    /// <summary>
    /// 分页条件
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    public interface IPagingCondition<TSource>
    {
        /// <summary>
        /// 正序
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        IPagingOrdered<TSource> OrderBy<TKey>(Expression<Func<TSource, TKey>> keySelector);

        /// <summary>
        /// 倒序
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        IPagingOrdered<TSource> OrderByDescending<TKey>(Expression<Func<TSource, TKey>> keySelector);

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IPagingCondition<TSource> Where(Expression<Func<TSource, bool>> predicate);
    }
}
