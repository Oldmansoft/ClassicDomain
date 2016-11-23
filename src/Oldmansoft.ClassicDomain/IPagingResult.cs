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
        /// 获取结果
        /// </summary>
        /// <param name="number">页码（从 1 开始）</param>
        /// <returns></returns>
        TSource[] GetResult(int number);

        /// <summary>
        /// 获取结果
        /// </summary>
        /// <param name="totalCount">总数</param>
        /// <param name="number">页码（从 1 开始）</param>
        /// <returns></returns>
        TSource[] GetResult(out int totalCount, int number);
    }
}
