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
    /// <typeparam name="TData"></typeparam>
    public interface IPageResult<TData>
    {
        /// <summary>
        /// 分页数据
        /// </summary>
        IList<TData> List { get; set; }

        /// <summary>
        /// 总数
        /// </summary>
        int TotalCount { get; set; }
    }
}
