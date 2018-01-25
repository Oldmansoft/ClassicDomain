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
        /// <param name="type">触发异常的类型</param>
        /// <param name="message">描述错误的消息</param>
        public ConfigItemException(Type type, string message)
            : base(type, message)
        { }
    }
}
