using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Core
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
        IDbSet<TDomain, TKey> Set<TDomain, TKey>();
    }
}
