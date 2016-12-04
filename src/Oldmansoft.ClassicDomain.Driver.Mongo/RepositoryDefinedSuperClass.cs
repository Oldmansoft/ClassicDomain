using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.Mongo
{
    /// <summary>
    /// 仓储库
    /// 定义父类
    /// </summary>
    /// <typeparam name="TDomain">领域类型</typeparam>
    /// <typeparam name="TSuperDomain">领域的父类</typeparam>
    /// <typeparam name="TKey">主键类型</typeparam>
    /// <typeparam name="TContext">领域上下文</typeparam>
    public class RepositoryDefinedSuperClass<TDomain, TSuperDomain, TKey, TContext> : Core.RepositoryDefinedSuperClass<TDomain, TSuperDomain, TKey>
        where TDomain : class, TSuperDomain
        where TSuperDomain : class
        where TContext : class, IContext, new()
    {
        /// <summary>
        /// 创建仓储
        /// </summary>
        /// <param name="uow"></param>
        public RepositoryDefinedSuperClass(UnitOfWork uow)
        {
            Context = uow.GetManaged<TContext>();
        }
    }

    /// <summary>
    /// 可传入初始化参数的仓储库
    /// 定义父类
    /// </summary>
    /// <typeparam name="TDomain">领域</typeparam>
    /// <typeparam name="TSuperDomain">领域的父类</typeparam>
    /// <typeparam name="TKey">主键</typeparam>
    /// <typeparam name="TContext">带初始化的上下文</typeparam>
    /// <typeparam name="TInit">初始化参数类型</typeparam>
    public class RepositoryDefinedSuperClass<TDomain, TSuperDomain, TKey, TContext, TInit> : Core.RepositoryDefinedSuperClass<TDomain, TSuperDomain, TKey>
        where TDomain : class, TSuperDomain
        where TSuperDomain : class
        where TContext : class, IContext<TInit>, new()
    {
        /// <summary>
        /// 创建可传入初始化参数的仓储库
        /// </summary>
        /// <param name="uow"></param>
        /// <param name="parameter"></param>
        public RepositoryDefinedSuperClass(UnitOfWork uow, TInit parameter)
        {
            Context = uow.GetManaged<TContext, TInit>(parameter);
        }
    }
}
