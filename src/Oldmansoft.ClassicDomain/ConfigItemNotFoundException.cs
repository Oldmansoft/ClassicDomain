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
        /// <param name="type">触发异常的类型</param>
        /// <param name="message">描述错误的消息</param>
        public ConfigItemNotFoundException(Type type, string message)
            : base(type, message)
        { }
    }
}
