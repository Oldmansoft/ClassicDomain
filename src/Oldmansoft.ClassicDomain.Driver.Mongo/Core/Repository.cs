using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Core
{
    /// <summary>
    /// 仓储库
    /// </summary>
    /// <typeparam name="TDomain">领域类型</typeparam>
    /// <typeparam name="TKey">主键类型</typeparam>
    public abstract class Repository<TDomain, TKey> : IRepository<TDomain, TKey>, IQuerySupport<TDomain>
        where TDomain : class
    {
        /// <summary>
        /// 上下文
        /// </summary>
        protected IContext Context { get; set; }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TDomain Get(TKey id)
        {
            return Context.Set<TDomain, TKey>().Get(id);
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
        /// 添加
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
        /// <param name="func"></param>
        protected void Execute(Func<MongoDB.Driver.MongoCollection<TDomain>, bool> func)
        {
            Context.Set<TDomain, TKey>().RegisterExecute(func);
        }

        /// <summary>
        /// 立即执行并返回结果
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        protected TResult Execute<TResult>(Func<MongoDB.Driver.MongoCollection<TDomain>, TResult> func)
        {
            return func(Context.Set<TDomain, TKey>().GetCollection());
        }
    }
}
