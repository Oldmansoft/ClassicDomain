using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain
{
    /// <summary>
    /// 配置项查找不到异常
    /// </summary>
    public class ConfigItemNotFoundException : ConfigItemException
    {
        /// <summary>
        /// 创建配置项查找不到异常
        /// </summary>
        /// <param name="message"></param>
        public ConfigItemNotFoundException(string message)
            : base(message)
        { }
    }
}
