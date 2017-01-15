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
    [Obsolete]
    public class PageResult<TData> : IPageResult<TData>
    {
        /// <summary>
        /// 分页数据
        /// </summary>
        public IList<TData> List { get; set; }

        /// <summary>
        /// 总数
        /// </summary>
        public int TotalCount { get; set; }
    }
}
