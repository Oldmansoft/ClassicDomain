using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Core
{
    /// <summary>
    /// 实体集
    /// </summary>
    /// <typeparam name="TDomain"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    internal class SafeModeDbSet<TDomain, TKey> : DbSet<TDomain, TKey>
    {
        private ChangeList<TDomain> List { get; set; }

        public IdentityMap<TDomain> IdentityMap { get; private set; }

        /// <summary>
        /// 创建实体集
        /// </summary>
        /// <param name="database"></param>
        /// <param name="keyExpression"></param>
        public SafeModeDbSet(MongoDatabase database, System.Linq.Expressions.Expression<Func<TDomain, TKey>> keyExpression)
            : base(database, keyExpression)
        {
            IdentityMap = new IdentityMap<TDomain>();
            IdentityMap.SetKey(keyExpression.Compile());
            List = new ChangeList<TDomain>();
        }

        /// <summary>
        /// 注册添加
        /// </summary>
        /// <param name="domain"></param>
        public override void RegisterAdd(TDomain domain)
        {
            TrySetDomainKey(domain);
            List.Addeds.Enqueue(domain);
            IdentityMap.Set(domain);
        }

        /// <summary>
        /// 注册替换
        /// </summary>
        /// <param name="domain"></param>
        public override void RegisterReplace(TDomain domain)
        {
            List.Updateds.Enqueue(domain);
        }

        /// <summary>
        /// 注册移除
        /// </summary>
        /// <param name="domain"></param>
        public override void RegisterRemove(TDomain domain)
        {
            List.Deleteds.Enqueue(domain);
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <returns></returns>
        public override int Commit()
        {
            var collection = GetCollection();

            int result = 0;
            TDomain domain;
            while (List.Addeds.TryDequeue(out domain))
            {
                if (Add(collection, domain)) result++;
            }
            while (List.Updateds.TryDequeue(out domain))
            {
                if (Replace(collection, domain)) result++;
            }
            while (List.Deleteds.TryDequeue(out domain))
            {
                if (Remove(collection, KeyExpressionCompile(domain))) result++;
            }

            return result + base.Commit();
        }

        /// <summary>
        /// 替换数据
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        protected override bool Replace(MongoCollection<TDomain> collection, TDomain domain)
        {
            var key = KeyExpressionCompile(domain);
            var source = IdentityMap.Get(key);
            if (source == null)
            {
                throw new ArgumentException("修改的实例必须经过加载。", "domain");
            }
            var context = Library.UpdateContext.GetContext(key, typeof(TDomain), source, domain);
            IdentityMap.Set(domain);
            if (!context.HasValue()) return false;
            return context.Execute(collection);
        }

        /// <summary>
        /// 移除数据
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        protected override bool Remove(MongoCollection<TDomain> collection, TKey id)
        {
            var result = base.Remove(collection, id);
            if (result) IdentityMap.Remove(id);
            return result;
        }
    }
}
