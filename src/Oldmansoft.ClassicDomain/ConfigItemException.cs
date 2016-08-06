using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain
{
    /// <summary>
    /// 配置项异常
    /// </summary>
    public class ConfigItemException : ClassicDomainException
    {
        /// <summary>
        /// 创建配置项异常
        /// </summary>
        /// <param name="message"></param>
        public ConfigItemException(string message)
            : base(message)
        { }
    }
}
