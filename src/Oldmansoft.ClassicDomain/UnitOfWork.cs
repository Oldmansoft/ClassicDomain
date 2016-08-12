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
        private static ConnectionNameMapHost HostMapping { get; set; }

        static UnitOfWork()
        {
            HostMapping = new ConnectionNameMapHost();
        }
        
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
            IsParallelCommit = true;
            UnitOfWorks = new Dictionary<Type, IUnitOfWorkManagedItem>();
        }

        /// <summary>
        /// 排序工作单元集
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private Dictionary<string, List<IUnitOfWorkManagedItem>> SortUnitOfWork(IEnumerable<IUnitOfWorkManagedItem> source)
        {
            Dictionary<string, List<IUnitOfWorkManagedItem>> result = new Dictionary<string, List<IUnitOfWorkManagedItem>>();
            foreach (var uow in source)
            {
                string host = HostMapping.GetHost(uow);
                if (!result.ContainsKey(host))
                {
                    result.Add(host, new List<IUnitOfWorkManagedItem>());
                }
                result[host].Add(uow);
            }
            return result;
        }

        /// <summary>
        /// 获取子工作单元
        /// </summary>
        /// <typeparam name="TUnitOfWork">工作单元类型</typeparam>
        /// <returns>工作单元</returns>
        public TUnitOfWork Get<TUnitOfWork>() where TUnitOfWork : class, IUnitOfWorkManagedItem, new()
        {
            Type type = typeof(TUnitOfWork);
            if (!UnitOfWorks.ContainsKey(type))
            {
                lock (UnitOfWorks)
                {
                    if (!UnitOfWorks.ContainsKey(type))
                    {
                        UnitOfWorks.Add(type, new TUnitOfWork());
                    }
                }
            }
            return UnitOfWorks[type] as TUnitOfWork;
        }
        
        private object Locker_CommitCount = new object();

        /// <summary>
        /// 将所有子工作单元提交
        /// </summary>
        /// <returns>受影响的数量</returns>
        public virtual int Commit()
        {
            int result = 0;
            try
            {
                if (UnitOfWorks.Count == 0) return 0;
                if (IsParallelCommit && UnitOfWorks.Count > 1)
                {
                    Parallel.ForEach(SortUnitOfWork(UnitOfWorks.Values), o =>
                    {
                        int count = 0;
                        foreach (var item in o.Value)
                        {
                            count += item.Commit();
                        }
                        lock (Locker_CommitCount)
                        {
                            result += count;
                        }
                    });
                }
                else
                {
                    foreach (var o in UnitOfWorks.Values)
                    {
                        result += o.Commit();
                    }
                }
            }
            catch (AggregateException ex)
            {
                if (OnCommitException != null)
                {
                    OnCommitException();
                }
                throw ex.InnerException;
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
