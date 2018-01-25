﻿using System;
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
        /// 初始化
        /// </summary>
        /// <param name="commands"></param>
        void Init(System.Collections.Concurrent.ConcurrentQueue<Driver.ICommand> commands);
        
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
