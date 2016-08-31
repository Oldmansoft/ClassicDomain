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
    /// <typeparam name="TDomain">领域</typeparam>
    /// <typeparam name="TKey">主键</typeparam>
    /// <typeparam name="TContext">上下文</typeparam>
    public class Repository<TDomain, TKey, TContext> : IRepository<TDomain, TKey> where TDomain : class where TContext : Context, new()
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
        /// 获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TDomain Get(TKey id)
        {
            return Context.Set<TDomain>().Find(id);
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
        /// 添加
        /// </summary>
        /// <param name="domain"></param>
        public void Add(TDomain domain)
        {
            if (domain == null) return;
            Context.Set<TDomain>().Add(domain);
        }

        /// <summary>
        /// 替换
        /// </summary>
        /// <param name="domain"></param>
        public void Replace(TDomain domain)
        {
            if (domain == null) return;
            if (Context.Entry(domain).State != System.Data.Entity.EntityState.Detached) return;
            Context.Set<TDomain>().Attach(domain);
            Context.Entry(domain).State = System.Data.Entity.EntityState.Modified;
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="domain"></param>
        public void Remove(TDomain domain)
        {
            if (domain == null) return;
            Context.Set<TDomain>().Remove(domain);
        }
        
        /// <summary>
        /// 立即执行并返回结果
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        protected TResult Execute<TResult>(Func<System.Data.Entity.DbSet<TDomain>, TResult> func)
        {
            return func(Context.Set<TDomain>());
        }
    }
}
