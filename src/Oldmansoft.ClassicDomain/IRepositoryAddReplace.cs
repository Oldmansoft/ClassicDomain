using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain
{
    /// <summary>
    /// 加载，查询，添加，替换
    /// </summary>
    /// <typeparam name="TDomain"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IRepositoryAddReplace<TDomain, TKey> :
        ILoad<TDomain, TKey>,
        IQuery<TDomain>,
        IAdd<TDomain>,
        IReplace<TDomain>,
        IRepositoryAdd<TDomain, TKey>
        where TDomain : class
    {
    }
}
