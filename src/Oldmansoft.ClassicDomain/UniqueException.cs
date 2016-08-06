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
        /// <param name="innerException"></param>
        public UniqueException(Exception innerException)
            : base(innerException.Message, innerException)
        { }

        /// <summary>
        /// 创建唯一约束异常
        /// </summary>
        /// <param name="message"></param>
        public UniqueException(string message)
            : base(message)
        { }
    }
}
