using System;
using System.Collections.Concurrent;

namespace Oldmansoft.ClassicDomain
{
    /// <summary>
    /// 工作单元
    /// </summary>
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        /// <summary>
        /// 管理项集
        /// </summary>
        private ConcurrentDictionary<Type, IUnitOfWorkManagedItem> ManagedItems { get; set; }

        /// <summary>
        /// 数据命令集
        /// </summary>
        private ConcurrentQueue<Driver.ICommand> Commands { get; set; }

        private bool Disposed;

        /// <summary>
        /// 创建工作单元
        /// </summary>
        public UnitOfWork()
        {
            ManagedItems = new ConcurrentDictionary<Type, IUnitOfWorkManagedItem>();
            Commands = new ConcurrentQueue<Driver.ICommand>();
        }

        /// <summary>
        /// 获取被管理的项
        /// </summary>
        /// <typeparam name="TManagedItem">工作单元管理项</typeparam>
        /// <returns>管理项</returns>
        public TManagedItem GetManaged<TManagedItem>() where TManagedItem : class, IUnitOfWorkManagedItem, new()
        {
            ThrowIfDisposed();
            Type type = typeof(TManagedItem);
            if (ManagedItems.TryGetValue(type, out IUnitOfWorkManagedItem result)) return result as TManagedItem;

            var context = new TManagedItem();
            context.Init(Commands);
            context.ModelCreating();
            ManagedItems.TryAdd(type, context);
            return context;
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <returns>受影响的数量</returns>
        public virtual int Commit()
        {
            ThrowIfDisposed();
            int result = 0;
            try
            {
                while (Commands.TryDequeue(out Driver.ICommand command))
                {
                    if (command.Execute())
                    {
                        result++;
                    }
                }
                foreach (var item in ManagedItems.Values)
                {
                    if (item is Driver.IUnitOfWorkFlushable flushable)
                    {
                        result += flushable.Flush();
                    }
                }
            }
            catch
            {
                while (Commands.TryDequeue(out _)) { }
                throw;
            }
            return result;
        }

        /// <summary>
        /// 释放上下文，并清空标识映射
        /// </summary>
        public void Dispose()
        {
            if (Disposed) return;
            Disposed = true;
            foreach (var item in ManagedItems.Values)
            {
                if (item is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            ManagedItems.Clear();
            while (Commands.TryDequeue(out _)) { }
        }

        private void ThrowIfDisposed()
        {
            if (Disposed) throw new ClassicDomainException(typeof(UnitOfWork), "工作单元已经释放。");
        }
    }
}
