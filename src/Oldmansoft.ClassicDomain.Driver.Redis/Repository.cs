using System;

namespace Oldmansoft.ClassicDomain.Driver.Redis
{
    /// <summary>
    /// 仓储
    /// </summary>
    /// <typeparam name="TDomain"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TContext">上下文</typeparam>
    public class Repository<TDomain, TKey, TContext> : IRepository<TDomain, TKey>
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
        /// 添加
        /// </summary>
        /// <param name="domain"></param>
        public void Add(TDomain domain)
        {
            if (domain == null) return;
            Context.Set<TDomain, TKey>().RegisterAdd(domain);
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
        /// 移除
        /// </summary>
        /// <param name="domain"></param>
        public void Remove(TDomain domain)
        {
            if (domain == null) return;
            Context.Set<TDomain, TKey>().RegisterRemove(domain);
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
        /// 执行
        /// </summary>
        /// <param name="func"></param>
        public void Execute(Func<StackExchange.Redis.IDatabase, bool> func)
        {
            Context.Set<TDomain, TKey>().RegisterExecute(func);
        }

        /// <summary>
        /// 立即执行并返回结果
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public TResult Execute<TResult>(Func<StackExchange.Redis.IDatabase, TResult> func)
        {
            return func(Context.Set<TDomain, TKey>().GetDatabase());
        }
    }
}
