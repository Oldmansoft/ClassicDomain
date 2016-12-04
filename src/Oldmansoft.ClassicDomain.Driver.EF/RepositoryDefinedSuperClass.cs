using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.EF
{
    /// <summary>
    /// 仓储库
    /// 定义父类
    /// </summary>
    /// <typeparam name="TDomain">领域类型</typeparam>
    /// <typeparam name="TSuperDomain">领域的父类</typeparam>
    /// <typeparam name="TKey">主键类型</typeparam>
    /// <typeparam name="TContext">领域上下文</typeparam>
    public class RepositoryDefinedSuperClass<TDomain, TSuperDomain, TKey, TContext> : Repository<TDomain, TKey, TContext>, IRepository<TSuperDomain, TKey>
        where TDomain : class, TSuperDomain
        where TSuperDomain : class
        where TContext : Context, new()
    {
        /// <summary>
        /// 创建仓储
        /// </summary>
        /// <param name="uow"></param>
        public RepositoryDefinedSuperClass(UnitOfWork uow)
            :base(uow)
        {
        }

        void IAdd<TSuperDomain>.Add(TSuperDomain domain)
        {
            Add(domain as TDomain);
        }

        TSuperDomain IGet<TSuperDomain, TKey>.Get(TKey id)
        {
            return Get(id);
        }

        IQueryable<TSuperDomain> IQuery<TSuperDomain>.Query()
        {
            return Query();
        }

        void IRemove<TSuperDomain>.Remove(TSuperDomain domain)
        {
            Remove(domain as TDomain);
        }

        void IReplace<TSuperDomain>.Replace(TSuperDomain domain)
        {
            Replace(domain as TDomain);
        }
    }
}
