using System;
using System.Linq;

namespace Oldmansoft.ClassicDomain.Driver.Mongo
{
    /// <summary>
    /// 仓储库
    /// </summary>
    /// <typeparam name="TDomain">领域类型</typeparam>
    /// <typeparam name="TKey">主键类型</typeparam>
    /// <typeparam name="TContext">领域上下文</typeparam>
    public class Repository<TDomain, TKey, TContext> : IRepository<TDomain, TKey>, IQuerySupport<TDomain>
        where TDomain : class
        where TContext : class, IContext, new()
    {
        /// <summary>
        /// 上下文
        /// </summary>
        protected IContext Context { get; set; }

        /// <summary>
        /// 设置工作单元
        /// </summary>
        /// <param name="uow"></param>
        public void SetUnitOfWork(UnitOfWork uow)
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
        /// 提交时执行
        /// </summary>
        /// <param name="func"></param>
        public void Execute(Func<MongoDB.Driver.MongoCollection<TDomain>, bool> func)
        {
            Context.Set<TDomain, TKey>().RegisterExecute(func);
        }

        /// <summary>
        /// 立即执行并返回结果
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public TResult Execute<TResult>(Func<MongoDB.Driver.MongoCollection<TDomain>, TResult> func)
        {
            return func(Context.Set<TDomain, TKey>().GetCollection());
        }
    }
}
