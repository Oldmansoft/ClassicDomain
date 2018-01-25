using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain
{
    /// <summary>
    /// 异常
    /// </summary>
    public class ClassicDomainException : Exception
    {
        /// <summary>
        /// 触发异常的类型
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// 创建异常
        /// </summary>
        /// <param name="type">触发异常的类型</param>
        public ClassicDomainException(Type type)
            : base()
        {
            Type = type;
        }

        /// <summary>
        /// 创建异常
        /// </summary>
        /// <param name="type">触发异常的类型</param>
        /// <param name="message">描述错误的消息</param>
        public ClassicDomainException(Type type, string message)
            : base(message)
        {
            Type = type;
        }

        /// <summary>
        /// 创建异常
        /// </summary>
        /// <param name="type">触发异常的类型</param>
        /// <param name="message">描述错误的消息</param>
        /// <param name="innerException">导致当前异常的异常</param>
        public ClassicDomainException(Type type, string message, Exception innerException)
            : base(message, innerException)
        {
            Type = type;
        }
    }
}
