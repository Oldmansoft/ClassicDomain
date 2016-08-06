using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain
{
    /// <summary>
    /// 移除
    /// </summary>
    public interface IRemove<TDomain> where TDomain : class
    {
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="domain"></param>
        void Remove(TDomain domain);
    }
}
