using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver
{
    /// <summary>
    /// 命令
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// 类型
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        bool Execute();
    }
}
