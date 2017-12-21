using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    /// <summary>
    /// 设值器
    /// </summary>
    public interface ISetter
    {
        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="value"></param>
        void Set(object caller, object value);
    }
}
