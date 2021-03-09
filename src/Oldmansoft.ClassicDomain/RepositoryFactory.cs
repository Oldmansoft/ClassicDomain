using System;
using System.Collections.Concurrent;

namespace Oldmansoft.ClassicDomain
{
    /// <summary>
    /// 仓储工厂
    /// </summary>
    public class RepositoryFactory : IRepositoryFactory
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

        /// <summary>
        /// 设置连接字符串
        /// </summary>
        /// <param name="contextType"></param>
        /// <param name="connectionString"></param>
        public static void SetConnectionString(Type contextType, string connectionString)
        {
            UnitOfWorkManagedContext.SetConnectionString(contextType, connectionString);
        }

        /// <summary>
        /// 设置连接字符串
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="connectionString"></param>
        public static void SetConnectionString<TContext>(string connectionString)
        {
            UnitOfWorkManagedContext.SetConnectionString(typeof(TContext), connectionString);
        }

        private readonly ConcurrentDictionary<Type, IRepository> RepositoryStore;

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
            return Uow;
        }
    }
}
