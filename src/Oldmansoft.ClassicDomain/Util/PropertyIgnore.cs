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
    class PropertyIgnore<TEntity> : IPropertyIgnore<TEntity>
    {
        private HashSet<string> Store { get; set; }

        /// <summary>
        /// 创建忽略属性
        /// </summary>
        /// <param name="store"></param>
        public PropertyIgnore(HashSet<string> store)
        {
            Store = store;
        }

        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public IPropertyIgnore<TEntity> Add(System.Linq.Expressions.Expression<Func<TEntity, object>> property)
        {
            Store.Add(property.GetPropertyContent());
            return this;
        }
    }
}
