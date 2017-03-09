using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    /// <summary>
    /// 忽略属性
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IPropertyIgnore<TEntity>
    {
        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        IPropertyIgnore<TEntity> Add(System.Linq.Expressions.Expression<Func<TEntity, object>> property);
    }
}
