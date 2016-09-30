using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Core
{
    /// <summary>
    /// 实体集
    /// </summary>
    /// <typeparam name="TDomain"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    internal class SafeModeDbSet<TDomain, TKey> : DbSet<TDomain, TKey>
    {
        public Core.IdentityMap<TDomain> IdentityMap { get; private set; }

        /// <summary>
        /// 创建实体集
        /// </summary>
        /// <param name="database"></param>
        /// <param name="keyExpression"></param>
        public SafeModeDbSet(MongoDB.Driver.MongoDatabase database, System.Linq.Expressions.Expression<Func<TDomain, TKey>> keyExpression)
            : base(database, keyExpression)
        {
            IdentityMap = new IdentityMap<TDomain>();
        }

        /// <summary>
        /// 替换数据
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        protected override bool Replace(MongoDB.Driver.MongoCollection<TDomain> collection, TDomain domain)
        {
            var key = IdentityMap.GetKey(domain);
            var source = IdentityMap.Get(key);
            if (source == null)
            {
                throw new ArgumentException("修改的实例必须经过加载。", "domain");
            }
            var context = Library.UpdateContext.GetContext(key, typeof(TDomain), source, domain);
            if (!context.HasValue()) return false;
            return context.Execute(collection);
        }

        /// <summary>
        /// 移除数据
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        protected override bool Remove(MongoDB.Driver.MongoCollection<TDomain> collection, TKey id)
        {
            IdentityMap.Remove(id);
            return base.Remove(collection, id);
        }
    }
}
