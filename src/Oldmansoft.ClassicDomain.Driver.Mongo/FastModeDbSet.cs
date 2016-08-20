using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Oldmansoft.ClassicDomain.Driver.Mongo
{
    /// <summary>
    /// 快速模式实体集
    /// </summary>
    /// <typeparam name="TDomain"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    internal class FastModeDbSet<TDomain, TKey> : IDbSet
    {
        private MongoDatabase Database { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        protected string TableName { get; private set; }

        private string OriginTableName { get; set; }

        private ChangeList<TDomain> List { get; set; }

        private ConcurrentQueue<Func<MongoCollection<TDomain>, bool>> ExecuteList { get; set; }

        private Func<TDomain, TKey> KeyExpressionCompile { get; set; }

        /// <summary>
        /// 主键表达式
        /// </summary>
        private System.Linq.Expressions.Expression<Func<TDomain, TKey>> KeyExpression { get; set; }

        /// <summary>
        /// 创建实体集
        /// </summary>
        /// <param name="database"></param>
        /// <param name="keyExpression"></param>
        public FastModeDbSet(MongoDatabase database, System.Linq.Expressions.Expression<Func<TDomain, TKey>> keyExpression)
        {
            Database = database;
            OriginTableName = typeof(TDomain).Name;
            TableName = OriginTableName;
            List = new ChangeList<TDomain>();
            ExecuteList = new ConcurrentQueue<Func<MongoCollection<TDomain>, bool>>();
            KeyExpression = keyExpression;
            KeyExpressionCompile = keyExpression.Compile();
        }

        /// <summary>
        /// 注册移除
        /// </summary>
        /// <param name="entity"></param>
        internal void RegisterRemove(TDomain entity)
        {
            List.Deleteds.Enqueue(entity);
        }

        /// <summary>
        /// 注册替换
        /// </summary>
        /// <param name="entity"></param>
        internal void RegisterReplace(TDomain entity)
        {
            List.Updateds.Enqueue(entity);
        }

        /// <summary>
        /// 注册添加
        /// </summary>
        /// <param name="entity"></param>
        internal void RegisterAdd(TDomain entity)
        {
            List.Addeds.Enqueue(entity);
        }

        /// <summary>
        /// 注册执行
        /// </summary>
        /// <param name="execute"></param>
        internal void RegisterExecute(Func<MongoCollection<TDomain>, bool> execute)
        {
            ExecuteList.Enqueue(execute);
        }

        /// <summary>
        /// 获取 Mongo 集
        /// </summary>
        /// <returns></returns>
        public MongoCollection<TDomain> GetCollection()
        {
            return Database.GetCollection<TDomain>(TableName);
        }
        
        /// <summary>
        /// 获取表名
        /// </summary>
        /// <returns></returns>
        internal string GetTableName()
        {
            return TableName;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        public IQueryable<TDomain> Query()
        {
            return GetCollection().AsQueryable();
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TDomain Get(TKey id)
        {
            return GetCollection().FindOneById(Library.Extend.ToBsonValue(id));
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <returns></returns>
        public int Commit()
        {
            var collection = GetCollection();

            int result = 0;
            TDomain entity;
            List<TDomain> addeds = new List<TDomain>();
            while (List.Addeds.TryDequeue(out entity))
            {
                addeds.Add(entity);
                result++;
            }
            if (addeds.Count > 0)
            {
                collection.InsertBatch(addeds);
            }

            while (List.Updateds.TryDequeue(out entity))
            {
                if (Replace(collection, entity)) result++;
            }
            while (List.Deleteds.TryDequeue(out entity))
            {
                if (Remove(collection, KeyExpressionCompile(entity))) result++;
            }

            Func<MongoCollection<TDomain>, bool> execute;
            while (ExecuteList.TryDequeue(out execute))
            {
                if (execute(collection)) result++;
            }
            return result;
        }

        /// <summary>
        /// 替换数据
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected virtual bool Replace(MongoCollection<TDomain> collection, TDomain entity)
        {
            var query = MongoDB.Driver.Builders.Query<TDomain>.EQ(KeyExpression, KeyExpressionCompile(entity));
            var update = MongoDB.Driver.Builders.Update<TDomain>.Replace(entity);
            return collection.Update(query, update).DocumentsAffected > 0;
        }

        /// <summary>
        /// 移除数据
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        protected virtual bool Remove(MongoCollection<TDomain> collection, TKey id)
        {
            var query = MongoDB.Driver.Builders.Query<TDomain>.EQ(KeyExpression, id);
            collection.Remove(query);
            return true;
        }
    }
}
