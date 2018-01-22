using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver
{
    /// <summary>
    /// 存储映射器
    /// </summary>
    public interface IStorageMapping
    {
        /// <summary>
        /// 获取映射
        /// </summary>
        /// <returns></returns>
        byte[] GetMapping();
    }
}
