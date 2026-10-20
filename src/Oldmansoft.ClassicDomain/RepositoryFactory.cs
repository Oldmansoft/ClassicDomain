using System;
using System.Collections.Concurrent;

namespace Oldmansoft.ClassicDomain
{
    /// <summary>
    /// 仓储工厂
    /// </summary>
    public class RepositoryFactory : IRepositoryFactory, IDisposable
    {
        private static readonly ConcurrentDictionary<Type, Func<UnitOfWork, IRepository>> Registers = new ConcurrentDictionary<Type, Func<UnitOfWork, IRepository>>();

        /// <summary>
        /// 注册
        /// </summary>
        /// <typeparam name="TRepository"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        public static void Add<TRepository, TImplementation>()
            where TRepository : IRepository
            where TImplementation : class, TRepository, new()
        {
            Registers.TryAdd(typeof(TRepository), (uow) =>
            {
                var result = new TImplementation();
                result.SetUnitOfWork(uow);
                return result;
            });
        }

        private readonly ConcurrentDictionary<Type, IRepository> RepositoryStore;

        private bool Disposed;

        /// <summary>
        /// 工作单元
        /// </summary>
        protected UnitOfWork Uow { get; private set; }

        /// <summary>
        /// 创建
        /// </summary>
        public RepositoryFactory()
        {
            RepositoryStore = new ConcurrentDictionary<Type, IRepository>();
            Uow = new UnitOfWork();
        }

        /// <summary>
        /// 获取仓储
        /// </summary>
        /// <typeparam name="TRepository"></typeparam>
        /// <returns></returns>
        public TRepository GetRepository<TRepository>() where TRepository : IRepository
        {
            ThrowIfDisposed();
            var type = typeof(TRepository);
            if (RepositoryStore.TryGetValue(type, out IRepository value))
            {
                return (TRepository)value;
            }
            if (!Registers.TryGetValue(type, out Func<UnitOfWork, IRepository> func))
            {
                throw new ClassicDomainException(type, string.Format("没有添加 {0} 的实现类，请用 RepositoryFactory.Add 静态方法添加。", type.FullName));
            }

            value = func(Uow);
            RepositoryStore.TryAdd(type, value);
            return (TRepository)value;
        }

        /// <summary>
        /// 获取工作单元
        /// </summary>
        /// <returns></returns>
        public UnitOfWork GetUnitOfWork()
        {
            ThrowIfDisposed();
            return Uow;
        }

        /// <summary>
        /// 释放内部工作单元
        /// </summary>
        public void Dispose()
        {
            if (Disposed) return;
            Disposed = true;
            Uow.Dispose();
            RepositoryStore.Clear();
        }

        private void ThrowIfDisposed()
        {
            if (Disposed) throw new ClassicDomainException(typeof(RepositoryFactory), "仓储工厂已经释放。");
        }
    }
}
