using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.EF
{
    /// <summary>
    /// Entity Framework 仓储
    /// </summary>
    /// <typeparam name="TDomain">领域类型</typeparam>
    /// <typeparam name="TKey">主键类型</typeparam>
    /// <typeparam name="TContext">领域上下文</typeparam>
    public class Repository<TDomain, TKey, TContext> : IRepository<TDomain, TKey>, IQuerySupport<TDomain>
        where TDomain : class
        where TContext : Context, new()
    {
        /// <summary>
        /// 实体上下文
        /// </summary>
        protected Context Context { get; set; }

        /// <summary>
        /// 创建仓储
        /// </summary>
        /// <param name="uow"></param>
        public Repository(UnitOfWork uow)
        {
            Context = uow.GetManaged<TContext>();
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        public IQueryable<TDomain> Query()
        {
            return Context.Set<TDomain>();
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TDomain Get(TKey id)
        {
            return Context.Set<TDomain>().Find(id);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="domain"></param>
        public void Add(TDomain domain)
        {
            if (domain == null) return;
            if (typeof(TKey) == typeof(Guid)) PrimaryKeyManager.Instance.TrySetDomainGuidEmptyKey(domain, Context);
            domain = domain.MapTo<TDomain>();
            Context.RegisterAdd(domain);
        }
        
        /// <summary>
        /// 替换
        /// </summary>
        /// <param name="domain"></param>
        public void Replace(TDomain domain)
        {
            if (domain == null) return;
            domain = domain.MapTo<TDomain>();
            Context.RegisterReplace(domain);
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="domain"></param>
        public void Remove(TDomain domain)
        {
            if (domain == null) return;
            Context.RegisterRemove(domain);
        }
        
        /// <summary>
        /// 立即执行并返回结果
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public TResult Execute<TResult>(Func<System.Data.Entity.DbSet<TDomain>, TResult> func)
        {
            return func(Context.Set<TDomain>());
        }
    }
}
