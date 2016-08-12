using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.Mongo
{
    /// <summary>
    /// 仓储库
    /// </summary>
    /// <typeparam name="TDomain"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class Repository<TDomain, TKey> : IRepository<TDomain, TKey> where TDomain : class
    {
        private FastModeContext Context { get; set; }

        /// <summary>
        /// 创建 Mongo 仓储库
        /// </summary>
        /// <param name="context">Mongo 上下文</param>
        public Repository(FastModeContext context)
        {
            Context = context;
        }

        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TDomain Load(TKey id)
        {
            return Context.Set<TDomain, TKey>().Load(id);
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
        /// 插入
        /// </summary>
        /// <param name="domain"></param>
        public void Add(TDomain domain)
        {
            if (domain == null) return;
            Context.Set<TDomain, TKey>().RegisterAdd(domain);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="domain"></param>
        public void Replace(TDomain domain)
        {
            if (domain == null) return;
            Context.Set<TDomain, TKey>().RegisterReplace(domain);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="domain"></param>
        public void Remove(TDomain domain)
        {
            if (domain == null) return;
            Context.Set<TDomain, TKey>().RegisterRemove(domain);
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="fun"></param>
        protected void Execute(Func<MongoDB.Driver.MongoCollection<TDomain>, bool> fun)
        {
            Context.Set<TDomain, TKey>().RegisterExecute(fun);
        }
    }
}
