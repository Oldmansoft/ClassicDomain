using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain
{
    /// <summary>
    /// 替换
    /// </summary>
    public interface IReplace<TDomain> where TDomain : class
    {
        /// <summary>
        /// 替换
        /// </summary>
        /// <param name="domain"></param>
        void Replace(TDomain domain);
    }
}
