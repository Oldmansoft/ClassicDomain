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
    public interface IUnitOfWorkManagedItem : IUnitOfWork
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

        /// <summary>
        /// 创建实体中
        /// </summary>
        void ModelCreating();
    }

    /// <summary>
    /// 可传入初始化参数的工作单元管理项
    /// </summary>
    /// <typeparam name="TInit">初始化参数类型</typeparam>
    public interface IUnitOfWorkManagedItem<TInit> : IUnitOfWorkManagedItem
    {
        /// <summary>
        /// 创建实体中
        /// </summary>
        /// <param name="parameter">初始化参数</param>
        void ModelCreating(TInit parameter);
    }
}
