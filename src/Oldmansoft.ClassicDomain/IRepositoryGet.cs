using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain
{
    /// <summary>
    /// 无查询
    /// </summary>
    /// <typeparam name="TDomain"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IRepositoryGet<TDomain, TKey> :
        IAdd<TDomain>,
        IReplace<TDomain>,
        IRemove<TDomain>,
        IGet<TDomain, TKey>
        where TDomain : class
    {
    }
}
