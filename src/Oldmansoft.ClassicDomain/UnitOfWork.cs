using System;
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
        /// 提交完成事件
        /// </summary>
        public event Action OnCommitCompleted;

        /// <summary>
        /// 提交异常事件
        /// </summary>
        public event Action OnCommitException;

        /// <summary>
        /// 是否并行提交
        /// </summary>
        [Obsolete("不再支持")]
        public bool IsParallelCommit { get; set; }

        /// <summary>
        /// 工作单元集
        /// </summary>
        private Dictionary<Type, IUnitOfWorkManagedItem> UnitOfWorks { get; set; }

        /// <summary>
        /// 创建工作单元管理
        /// </summary>
        public UnitOfWork()
        {
            UnitOfWorks = new Dictionary<Type, IUnitOfWorkManagedItem>();
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
            int result = 0;
            if (UnitOfWorks.Count == 0) return 0;
            try
            {
                foreach (var o in UnitOfWorks.Values)
                {
                    result += o.Commit();
                }
            }
            catch
            {
                if (OnCommitException != null)
                {
                    OnCommitException();
                }
                throw;
            }

            if (OnCommitCompleted != null)
            {
                OnCommitCompleted();
            }
            return result;
        }
    }
}
