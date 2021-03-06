﻿using Oldmansoft.ClassicDomain.Driver;
using System.Collections.Concurrent;

namespace Oldmansoft.ClassicDomain
{
    /// <summary>
    /// 工作单元管理项
    /// </summary>
    public abstract class UnitOfWorkManagedContext : IUnitOfWorkManagedItem
    {
        /// <summary>
        /// 更新命令集
        /// </summary>
        protected ConcurrentQueue<ICommand> Commands { get; private set; }

        void IUnitOfWorkManagedItem.Init(ConcurrentQueue<ICommand> commands)
        {
            Commands = commands;
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
    }
}
