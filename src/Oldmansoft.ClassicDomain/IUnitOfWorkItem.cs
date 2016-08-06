using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain
{
    /// <summary>
    /// 工作单元管理项
    /// </summary>
    public interface IUnitOfWorkItem : IUnitOfWork
    {
        /// <summary>
        /// 获取配置读取的连接串名称
        /// </summary>
        /// <returns></returns>
        string GetConnectionName();

        /// <summary>
        /// 获取 Commit 的主机
        /// </summary>
        /// <returns></returns>
        string GetHost();
    }
}
