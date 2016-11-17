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
    public abstract class UnitOfWorkManagedItem : IUnitOfWorkManagedItem
    {
        /// <summary>
        /// 配置读取的连接串名称
        /// </summary>
        protected string ConnectionName { get; set; }

        /// <summary>
        /// 创建工作单元管理项
        /// </summary>
        public UnitOfWorkManagedItem()
        {
            ConnectionName = GetType().FullName;
        }

        /// <summary>
        /// 获取配置读取的连接串名称
        /// </summary>
        /// <returns></returns>
        public string GetConnectionName()
        {
            return ConnectionName;
        }

        /// <summary>
        /// 创建实体中，此方法由 UnitOfWork 调用
        /// </summary>
        void IUnitOfWorkManagedItem.ModelCreating()
        {
            OnModelCreating();
        }

        /// <summary>
        /// 在创建实体时
        /// </summary>
        protected abstract void OnModelCreating();

        /// <summary>
        /// 提交
        /// </summary>
        /// <returns></returns>
        public abstract int Commit();
    }
}
