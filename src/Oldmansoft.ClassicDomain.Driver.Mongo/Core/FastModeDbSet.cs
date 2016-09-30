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
        /// <summary>
        /// 创建实体集
        /// </summary>
        /// <param name="database"></param>
        /// <param name="keyExpression"></param>
        public FastModeDbSet(MongoDatabase database, System.Linq.Expressions.Expression<Func<TDomain, TKey>> keyExpression)
            : base(database, keyExpression)
        {
        }

        /// <summary>
        /// 替换数据
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected override bool Replace(MongoCollection<TDomain> collection, TDomain entity)
        {
            var query = MongoDB.Driver.Builders.Query<TDomain>.EQ(KeyExpression, KeyExpressionCompile(entity));
            var update = MongoDB.Driver.Builders.Update<TDomain>.Replace(entity);
            var writeResult = collection.Update(query, update);
            if (writeResult == null) return true;
            return writeResult.DocumentsAffected > 0;
        }
    }
}
