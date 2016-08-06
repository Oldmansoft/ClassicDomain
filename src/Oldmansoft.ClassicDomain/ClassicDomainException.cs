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
        /// 创建异常
        /// </summary>
        public ClassicDomainException()
            : base()
        { }

        /// <summary>
        /// 创建异常
        /// </summary>
        /// <param name="message">描述错误的消息</param>
        public ClassicDomainException(string message)
            : base(message)
        { }

        /// <summary>
        /// 创建异常
        /// </summary>
        /// <param name="message">描述错误的消息</param>
        /// <param name="innerException">导致当前异常的异常</param>
        public ClassicDomainException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
