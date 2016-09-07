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
        /// 在创建实体时
        /// </summary>
        void OnModelCreating();
    }

    /// <summary>
    /// 工作单元管理项
    /// </summary>
    /// <typeparam name="TInit">初始化参数类型</typeparam>
    public interface IUnitOfWorkManagedItem<TInit> : IUnitOfWorkManagedItem
    {
        /// <summary>
        /// 初始化方法，此方法由 UnitOfWork 调用
        /// </summary>
        /// <param name="parameter">初始化参数</param>
        void OnModelCreating(TInit parameter);
    }
}
