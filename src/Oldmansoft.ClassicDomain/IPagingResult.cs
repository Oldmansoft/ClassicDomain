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

        /// <summary>
        /// 从上一页最后一条之后继续取一页。last 为空时取第一页。
        /// 条件写在排序键上，不再跳过前面的行。
        /// </summary>
        /// <param name="last">上一页的最后一条</param>
        /// <returns></returns>
        IList<TSource> ToListAfter(TSource last);
    }
}
