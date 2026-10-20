using Oldmansoft.ClassicDomain.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Driver;

namespace Oldmansoft.ClassicDomain.Driver.Mongo
{
    /// <summary>
    /// 设置
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TMember"></typeparam>
    public class Setting<TEntity, TMember>
    {
        private static OnceSet<string> OnceSet { get; set; }

        static Setting()
        {
            OnceSet = new OnceSet<string>();
        }

        private Core.IDbSet<TEntity, TMember> DbSet { get; set; }

        internal Setting(Core.IDbSet<TEntity, TMember> dbSet)
        {
            DbSet = dbSet;
        }

        private string GetMember(Expression<Func<TEntity, object>> keyExpression)
        {
            return keyExpression.GetPropertyFullName();
        }

        private string[] GetNames(Expression<Func<TEntity, object>>[] keyExpressions)
        {
            string[] keyNames = new string[keyExpressions.Length];
            for (int i = 0; i < keyExpressions.Length; i++)
            {
                keyNames[i] = GetMember(keyExpressions[i]);
            }
            return keyNames;
        }

        /// <summary>
        /// 定义实体的映射数据表名
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public Setting<TEntity, TMember> ToTable(string tableName)
        {
            DbSet.SetTableName(tableName);
            return this;
        }

        /// <summary>
        /// 设置索引
        /// </summary>
        /// <param name="keyExpression">索引键表达式</param>
        /// <returns>设置</returns>
        public Setting<TEntity, TMember> SetIndex(Expression<Func<TEntity, object>> keyExpression)
        {
            return SetIndex(true, false, keyExpression);
        }

        /// <summary>
        /// 设置索引
        /// </summary>
        /// <param name="keyExpression">索引键表达式</param>
        /// <returns>设置</returns>
        public Setting<TEntity, TMember> SetIndex(Func<TEntity, ExpressionGroup<TEntity>> keyExpression)
        {
            return SetIndex(keyExpression(default));
        }

        /// <summary>
        /// 设置索引
        /// </summary>
        /// <param name="keyExpression">索引键表达式</param>
        /// <returns>设置</returns>
        public Setting<TEntity, TMember> SetIndex(ExpressionGroup<TEntity> keyExpression)
        {
            if (keyExpression == null) return this;
            if (keyExpression.Expressions.Count == 0) return this;
            return SetIndex(true, false, keyExpression.Expressions.ToArray());
        }

        /// <summary>
        /// 设置倒序索引
        /// </summary>
        /// <param name="keyExpression">索引键表达式</param>
        /// <returns>设置</returns>
        public Setting<TEntity, TMember> SetIndexDescending(Expression<Func<TEntity, object>> keyExpression)
        {
            return SetIndex(false, false, keyExpression);
        }

        /// <summary>
        /// 设置倒序索引
        /// </summary>
        /// <param name="keyExpression">索引键表达式</param>
        /// <returns>设置</returns>
        public Setting<TEntity, TMember> SetIndexDescending(Func<TEntity, ExpressionGroup<TEntity>> keyExpression)
        {
            return SetIndexDescending(keyExpression(default));
        }

        /// <summary>
        /// 设置倒序索引
        /// </summary>
        /// <param name="keyExpression">索引键表达式</param>
        /// <returns>设置</returns>
        public Setting<TEntity, TMember> SetIndexDescending(ExpressionGroup<TEntity> keyExpression)
        {
            if (keyExpression == null) return this;
            if (keyExpression.Expressions.Count == 0) return this;
            return SetIndex(false, false, keyExpression.Expressions.ToArray());
        }

        /// <summary>
        /// 设置唯一索引
        /// </summary>
        /// <param name="keyExpression">索引键表达式</param>
        /// <returns>设置</returns>
        public Setting<TEntity, TMember> SetUnique(Expression<Func<TEntity, object>> keyExpression)
        {
            return SetIndex(true, true, keyExpression);
        }

        /// <summary>
        /// 设置唯一索引
        /// </summary>
        /// <param name="keyExpression">索引键表达式</param>
        /// <returns>设置</returns>
        public Setting<TEntity, TMember> SetUnique(Func<TEntity, ExpressionGroup<TEntity>> keyExpression)
        {
            return SetUnique(keyExpression(default));
        }

        /// <summary>
        /// 设置唯一索引
        /// </summary>
        /// <param name="keyExpression">索引键表达式</param>
        /// <returns>设置</returns>
        public Setting<TEntity, TMember> SetUnique(ExpressionGroup<TEntity> keyExpression)
        {
            if (keyExpression == null) return this;
            if (keyExpression.Expressions.Count == 0) return this;
            return SetIndex(true, true, keyExpression.Expressions.ToArray());
        }

        /// <summary>
        /// 设置倒序唯一索引
        /// </summary>
        /// <param name="keyExpression">索引键表达式</param>
        /// <returns>设置</returns>
        public Setting<TEntity, TMember> SetUniqueDescending(Expression<Func<TEntity, object>> keyExpression)
        {
            return SetIndex(false, true, keyExpression);
        }

        /// <summary>
        /// 设置倒序唯一索引
        /// </summary>
        /// <param name="keyExpression">索引键表达式</param>
        /// <returns>设置</returns>
        public Setting<TEntity, TMember> SetUniqueDescending(Func<TEntity, ExpressionGroup<TEntity>> keyExpression)
        {
            return SetUniqueDescending(keyExpression(default));
        }

        /// <summary>
        /// 设置倒序唯一索引
        /// </summary>
        /// <param name="keyExpression">索引键表达式</param>
        /// <returns>设置</returns>
        public Setting<TEntity, TMember> SetUniqueDescending(ExpressionGroup<TEntity> keyExpression)
        {
            if (keyExpression == null) return this;
            if (keyExpression.Expressions.Count == 0) return this;
            return SetIndex(false, true, keyExpression.Expressions.ToArray());
        }

        /// <summary>
        /// 判断索引存在
        /// </summary>
        /// <param name="indexs"></param>
        /// <param name="keyNames"></param>
        /// <param name="direction">1 为升序，-1 为降序；空表示不比较方向</param>
        /// <param name="unique">空表示不比较唯一约束</param>
        /// <returns></returns>
        private bool IndexExists(IMongoIndexManager<TEntity> indexs, string[] keyNames, int? direction, bool? unique)
        {
            foreach (var bson in indexs.List().ToList())
            {
                var key = bson["key"].AsBsonDocument;
                if (key.ElementCount != keyNames.Length) continue;
                var matched = true;
                var index = 0;
                foreach (var element in key)
                {
                    if (element.Name != keyNames[index])
                    {
                        matched = false;
                        break;
                    }
                    if (direction.HasValue && (!element.Value.IsNumeric || element.Value.ToInt32() != direction.Value))
                    {
                        matched = false;
                        break;
                    }
                    index++;
                }
                if (!matched) continue;
                if (unique.HasValue)
                {
                    var isUnique = bson.Contains("unique") && bson["unique"].ToBoolean();
                    if (isUnique != unique.Value) continue;
                }
                return true;
            }
            return false;
        }

        private static string StableIndexName(string tableName, string[] keyNames, bool isAscending, bool isUnique)
        {
            return tableName + "_" + string.Join("_", keyNames) + "_" + (isAscending ? "asc" : "desc") + (isUnique ? "_unique" : "");
        }

        /// <summary>
        /// 设置索引
        /// </summary>
        /// <param name="isAscending">是否顺序排序</param>
        /// <param name="isUnique">是否唯一</param>
        /// <param name="keyExpressions">索引键表达式</param>
        /// <returns>设置</returns>
        private Setting<TEntity, TMember> SetIndex(bool isAscending, bool isUnique, params Expression<Func<TEntity, object>>[] keyExpressions)
        {
            if (keyExpressions == null || keyExpressions.Length == 0) return this;
            string[] keyNames = GetNames(keyExpressions);
            var tableName = DbSet.GetTableName();
            if (!OnceSet.Use(string.Format("SetIndex:{0}:{1}:{2}:{3}", tableName, string.Join(",", keyNames), isAscending, isUnique))) return this;

            var collection = DbSet.GetCollection();
            if (!IndexExists(collection.Indexes, keyNames, isAscending ? 1 : -1, isUnique))
            {
                var keys = new List<IndexKeysDefinition<TEntity>>();
                if (isAscending)
                {
                    foreach (var keyExpression in keyExpressions)
                    {
                        keys.Add(Builders<TEntity>.IndexKeys.Ascending(keyExpression));
                    }
                }
                else
                {
                    foreach (var keyExpression in keyExpressions)
                    {
                        keys.Add(Builders<TEntity>.IndexKeys.Descending(keyExpression));
                    }
                }
                var options = new CreateIndexOptions { Name = StableIndexName(tableName, keyNames, isAscending, isUnique) };
                if (isUnique) options.Unique = true;
                collection.Indexes.CreateOne(new CreateIndexModel<TEntity>(Builders<TEntity>.IndexKeys.Combine(keys), options));
            }
            return this;
        }

        /// <summary>
        /// 创建 2d 索引
        /// </summary>
        /// <param name="keyExpression"></param>
        /// <returns>设置</returns>
        public Setting<TEntity, TMember> SetGeoSpatial(Expression<Func<TEntity, object>> keyExpression)
        {
            var key = GetMember(keyExpression);
            if (!OnceSet.Use(string.Format("SetIndex{0}:{1}", DbSet.GetTableName(), key))) return this;

            var collection = DbSet.GetCollection();
            if (!IndexExists(collection.Indexes, new string[] { key }, null, null))
            {
                var model = new CreateIndexModel<TEntity>(Builders<TEntity>.IndexKeys.Geo2D(keyExpression));
                collection.Indexes.CreateOne(model);
            }
            return this;
        }

        /// <summary>
        /// 创建 2dsphere 索引
        /// </summary>
        /// <param name="keyExpression"></param>
        /// <returns>设置</returns>
        public Setting<TEntity, TMember> SetGeoSpatialSpherical(Expression<Func<TEntity, object>> keyExpression)
        {
            var key = GetMember(keyExpression);
            if (!OnceSet.Use(string.Format("SetIndex{0}:{1}", DbSet.GetTableName(), key))) return this;

            var collection = DbSet.GetCollection();
            if (!IndexExists(collection.Indexes, new string[] { key }, null, null))
            {
                var model = new CreateIndexModel<TEntity>(Builders<TEntity>.IndexKeys.Geo2DSphere(keyExpression));
                collection.Indexes.CreateOne(model);
            }
            return this;
        }
    }
}
