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
    /// <typeparam name="TDomain">领域</typeparam>
    /// <typeparam name="TKey">主键</typeparam>
    /// <typeparam name="TContext">上下文</typeparam>
    public class Repository<TDomain, TKey, TContext> : IRepository<TDomain, TKey> where TDomain : class where TContext : class, IContext, new()
    {
        /// <summary>
        /// 上下文
        /// </summary>
        protected IContext Context { get; set; }

        /// <summary>
        /// 创建仓储
        /// </summary>
        protected Repository()
        {
        }

        /// <summary>
        /// 创建仓储
        /// </summary>
        /// <param name="uow"></param>
        public Repository(UnitOfWork uow)
        {
            if (typeof(TContext).IsGenericType) throw new ArgumentNullException("不能用此类调用泛型类型");
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

    /// <summary>
    /// 仓储库
    /// </summary>
    /// <typeparam name="TDomain">领域</typeparam>
    /// <typeparam name="TKey">主键</typeparam>
    /// <typeparam name="TContext">上下文</typeparam>
    /// <typeparam name="TInit"></typeparam>
    public class Repository<TDomain, TKey, TContext, TInit> : Repository<TDomain, TKey, TContext> where TDomain : class where TContext : class, IContext<TInit>, new()
    {
        /// <summary>
        /// 上下文
        /// </summary>
        protected IContext<TInit> InitContext { get; set; }

        /// <summary>
        /// 创建仓储
        /// </summary>
        /// <param name="uow"></param>
        /// <param name="parameter"></param>
        public Repository(UnitOfWork uow, TInit parameter)
        {
            InitContext = uow.GetManaged<TContext, TInit>(parameter);
            Context = InitContext;
        }
    }
}
