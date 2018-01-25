using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain
{
    /// <summary>
    /// 工作单元管理
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// 工作单元集
        /// </summary>
        private Dictionary<Type, IUnitOfWorkManagedItem> UnitOfWorks { get; set; }

        /// <summary>
        /// 数据命令集
        /// </summary>
        private ConcurrentQueue<Driver.ICommand> Commands { get; set; }

        /// <summary>
        /// 创建工作单元管理
        /// </summary>
        public UnitOfWork()
        {
            UnitOfWorks = new Dictionary<Type, IUnitOfWorkManagedItem>();
            Commands = new ConcurrentQueue<Driver.ICommand>();
        }
        
        /// <summary>
        /// 获取被管理的子工作单元
        /// </summary>
        /// <typeparam name="TUnitOfWork">工作单元类型</typeparam>
        /// <returns>工作单元</returns>
        public TUnitOfWork GetManaged<TUnitOfWork>() where TUnitOfWork : class, IUnitOfWorkManagedItem, new()
        {
            Type type = typeof(TUnitOfWork);
            if (!UnitOfWorks.ContainsKey(type))
            {
                var context = new TUnitOfWork();
                context.Init(Commands);
                context.ModelCreating();
                lock (UnitOfWorks)
                {
                    if (!UnitOfWorks.ContainsKey(type))
                    {
                        UnitOfWorks.Add(type, context);
                    }
                }
            }
            return UnitOfWorks[type] as TUnitOfWork;
        }

        /// <summary>
        /// 获取被管理的子工作单元
        /// </summary>
        /// <typeparam name="TUnitOfWork">工作单元类型</typeparam>
        /// <typeparam name="TInit"></typeparam>
        /// <returns>工作单元</returns>
        public TUnitOfWork GetManaged<TUnitOfWork, TInit>(TInit parameter) where TUnitOfWork : class, IUnitOfWorkManagedItem<TInit>, new()
        {
            Type type = typeof(TUnitOfWork);
            if (!UnitOfWorks.ContainsKey(type))
            {
                var context = new TUnitOfWork();
                context.Init(Commands);
                context.ModelCreating(parameter);
                lock (UnitOfWorks)
                {
                    if (!UnitOfWorks.ContainsKey(type))
                    {
                        UnitOfWorks.Add(type, context);
                    }
                }
            }
            return UnitOfWorks[type] as TUnitOfWork;
        }

        /// <summary>
        /// 将所有子工作单元提交
        /// </summary>
        /// <returns>受影响的数量</returns>
        public virtual int Commit()
        {
            if (UnitOfWorks.Count == 0) return 0;
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
