using System;
using System.Collections.Generic;
using System.Text;

namespace Oldmansoft.ClassicDomain
{
    /// <summary>
    /// 工作单元
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// 提交
        /// </summary>
        /// <returns>更新数</returns>
        int Commit();
    }
}
