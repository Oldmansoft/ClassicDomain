using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Core
{
    /// <summary>
    /// 仓储
    /// </summary>
    /// <typeparam name="TDomain"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public abstract class Repository<TDomain, TKey> : IRepositoryGet<TDomain, TKey>
        where TDomain : class
    {
        /// <summary>
        /// 上下文
        /// </summary>
        protected IContext Context { get; set; }
        
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="domain"></param>
        public void Add(TDomain domain)
        {
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
            Context.Set<TDomain, TKey>().RegisterRemove(domain);
        }

        /// <summary>
        /// 替换
        /// </summary>
        /// <param name="domain"></param>
        public void Replace(TDomain domain)
        {
            Context.Set<TDomain, TKey>().RegisterReplace(domain);
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="func"></param>
        protected void Execute(Func<StackExchange.Redis.IDatabase, bool> func)
        {
            Context.Set<TDomain, TKey>().RegisterExecute(func);
        }

        /// <summary>
        /// 立即执行并返回结果
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        protected TResult Execute<TResult>(Func<StackExchange.Redis.IDatabase, TResult> func)
        {
            return func(Context.Set<TDomain, TKey>().GetDatabase());
        }
    }
}
