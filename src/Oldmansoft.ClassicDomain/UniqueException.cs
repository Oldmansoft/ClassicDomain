using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain
{
    /// <summary>
    /// 唯一约束异常
    /// </summary>
    public class UniqueException : ClassicDomainException
    {
        /// <summary>
        /// 创建唯一约束异常
        /// </summary>
        /// <param name="type">触发异常的类型</param>
        /// <param name="innerException">导致当前异常的异常</param>
        public UniqueException(Type type, Exception innerException)
            : base(type, innerException.Message, innerException)
        { }

        /// <summary>
        /// 创建唯一约束异常
        /// </summary>
        /// <param name="type">触发异常的类型</param>
        /// <param name="message">描述错误的消息</param>
        public UniqueException(Type type, string message)
            : base(type, message)
        { }
    }
}
