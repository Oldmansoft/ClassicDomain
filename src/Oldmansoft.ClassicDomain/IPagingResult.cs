using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
