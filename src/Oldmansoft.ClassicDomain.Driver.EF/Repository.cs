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
    /// <typeparam name="TDomain"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class Repository<TDomain, TKey> : IRepository<TDomain, TKey> where TDomain : class
    {
        /// <summary>
        /// 实体上下文
        /// </summary>
        protected Context Context { get; set; }

        /// <summary>
        /// 创建仓储
        /// </summary>
        /// <param name="context"></param>
        public Repository(Context context)
        {
            Context = context;
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
    }
}
