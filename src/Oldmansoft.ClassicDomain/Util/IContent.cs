using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    /// <summary>
    /// 内容
    /// </summary>
    public interface IContent
    {
        /// <summary>
        /// 类型
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; }
    }
}
