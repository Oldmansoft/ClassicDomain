using System;
using System.Collections.Generic;
using System.Text;

namespace Oldmansoft.ClassicDomain.Driver.Mongo
{
    /// <summary>
    /// 实体上下文接口
    /// </summary>
    public interface IContext : IUnitOfWorkManagedItem
    {
        /// <summary>
        /// 获取实体集
        /// </summary>
        /// <typeparam name="TDomain"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        Core.IDbSet<TDomain, TKey> Set<TDomain, TKey>();
    }
}
