using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain
{
    /// <summary>
    /// 查询
    /// </summary>
    public interface IQuery<TDomain> where TDomain : class
    {
        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        IQueryable<TDomain> Query();
    }
}
