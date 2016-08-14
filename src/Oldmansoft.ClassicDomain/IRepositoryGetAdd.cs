using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain
{
    /// <summary>
    /// 获取，添加
    /// </summary>
    /// <typeparam name="TDomain"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IRepositoryGetAdd<TDomain, TKey> :
        IGet<TDomain, TKey>,
        IAdd<TDomain>
        where TDomain : class
    {
    }
}
