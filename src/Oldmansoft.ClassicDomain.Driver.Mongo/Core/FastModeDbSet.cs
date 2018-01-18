using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Core
{
    /// <summary>
    /// 快速模式实体集
    /// </summary>
    /// <typeparam name="TDomain"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    internal class FastModeDbSet<TDomain, TKey> : DbSet<TDomain, TKey>
    {
        private ChangeList<TDomain> List { get; set; }

        /// <summary>
        /// 创建实体集
        /// </summary>
        /// <param name="database"></param>
        /// <param name="keyExpression"></param>
        public FastModeDbSet(MongoDatabase database, System.Linq.Expressions.Expression<Func<TDomain, TKey>> keyExpression)
            : base(database, keyExpression)
        {
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
                if (Remove(collection, domain)) result++;
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
            var query = MongoDB.Driver.Builders.Query<TDomain>.EQ(KeyExpression, KeyExpressionCompile(domain));
            var update = MongoDB.Driver.Builders.Update<TDomain>.Replace(domain);
            var writeResult = collection.Update(query, update);
            if (writeResult == null) return true;
            return writeResult.DocumentsAffected > 0;
        }
    }
}
