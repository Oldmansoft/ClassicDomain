using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain
{
    /// <summary>
    /// 加载
    /// </summary>
    /// <typeparam name="TDomain"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface ILoad<TDomain, TKey> where TDomain : class
    {
        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TDomain Load(TKey id);

        /// <summary>
        /// 加载多个
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        TDomain[] Load(TKey[] ids);
    }
}
