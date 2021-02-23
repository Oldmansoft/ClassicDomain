using System.Linq;

namespace Oldmansoft.ClassicDomain
{
    /// <summary>
    /// 查询支持
    /// </summary>
    public interface IQuerySupport<TDomain> where TDomain : class
    {
        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        IQueryable<TDomain> Query();
    }
}
