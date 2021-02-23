using System.Linq;

namespace Oldmansoft.ClassicDomain.Driver.InProcess
{
    /// <summary>
    /// 进程内仓储
    /// </summary>
    /// <typeparam name="TDomain">领域</typeparam>
    /// <typeparam name="TKey">主键</typeparam>
    /// <typeparam name="TContext">上下文</typeparam>
    public class Repository<TDomain, TKey, TContext> : IRepository<TDomain, TKey>
        where TDomain : class
        where TContext : Context, new()
    {
        private Context Context { get; set; }

        /// <summary>
        /// 设置工作单元
        /// </summary>
        /// <param name="uow"></param>
        public void SetUnitOfWork(UnitOfWork uow)
        {
            Context = uow.GetManaged<TContext>();
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        public IQueryable<TDomain> Query()
        {
            return Context.Set<TDomain, TKey>().Query();
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        public TDomain Get(TKey id)
        {
            return Context.Set<TDomain, TKey>().Get(id);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="domain"></param>
        public void Add(TDomain domain)
        {
            if (domain == null) return;
            Context.Set<TDomain, TKey>().RegisterAdd(domain);
        }

        /// <summary>
        /// 替换
        /// </summary>
        /// <param name="domain"></param>
        public void Replace(TDomain domain)
        {
            if (domain == null) return;
            Context.Set<TDomain, TKey>().RegisterReplace(domain);
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="domain"></param>
        public void Remove(TDomain domain)
        {
            if (domain == null) return;
            Context.Set<TDomain, TKey>().RegisterRemove(domain);
        }
    }
}
