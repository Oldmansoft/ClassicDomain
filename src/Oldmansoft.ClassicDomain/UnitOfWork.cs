using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain
{
    /// <summary>
    /// 工作单元
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// 管理项集
        /// </summary>
        private ConcurrentDictionary<Type, IUnitOfWorkManagedItem> ManagedItems { get; set; }

        /// <summary>
        /// 数据命令集
        /// </summary>
        private ConcurrentQueue<Driver.ICommand> Commands { get; set; }

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
            Type type = typeof(TManagedItem);
            IUnitOfWorkManagedItem result;
            if (ManagedItems.TryGetValue(type, out result)) return result as TManagedItem;

            var context = new TManagedItem();
            context.Init(Commands);
            context.ModelCreating();
            ManagedItems.TryAdd(type, context);
            return context;
        }

        /// <summary>
        /// 获取被管理的项
        /// </summary>
        /// <typeparam name="TManagedItem">工作单元管理项</typeparam>
        /// <typeparam name="TInit"></typeparam>
        /// <returns>管理项</returns>
        public TManagedItem GetManaged<TManagedItem, TInit>(TInit parameter) where TManagedItem : class, IUnitOfWorkManagedItem<TInit>, new()
        {
            Type type = typeof(TManagedItem);
            IUnitOfWorkManagedItem result;
            if (ManagedItems.TryGetValue(type, out result)) return result as TManagedItem;

            var context = new TManagedItem();
            context.Init(Commands);
            context.ModelCreating(parameter);
            ManagedItems.TryAdd(type, context);
            return context;
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <returns>受影响的数量</returns>
        public virtual int Commit()
        {
            int result = 0;
            Driver.ICommand command;
            while (Commands.TryDequeue(out command))
            {
                if (command.Execute()) result++;
            }
            return result;
        }
    }
}
