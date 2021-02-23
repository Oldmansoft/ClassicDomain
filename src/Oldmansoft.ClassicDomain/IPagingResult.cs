using System.Collections.Generic;

namespace Oldmansoft.ClassicDomain
{
    /// <summary>
    /// 分页结果
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    public interface IPagingResult<TSource>
    {
        /// <summary>
        /// 生成列表
        /// </summary>
        /// <param name="number">页码（从 1 开始）</param>
        /// <returns></returns>
        IList<TSource> ToList(int number);

        /// <summary>
        /// 生成列表
        /// </summary>
        /// <param name="number">页码（从 1 开始）</param>
        /// <param name="totalCount">所有记录数</param>
        /// <returns></returns>
        IList<TSource> ToList(int number, out int totalCount);
    }
}
