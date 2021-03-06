﻿using MongoDB.Driver.Builders;
using Oldmansoft.ClassicDomain.Util;
using System;
using System.Linq.Expressions;

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
            if (!OnceSet.Use(string.Format("SetIndex{0}:{1}", DbSet.GetTableName(), string.Join(",", keyNames)))) return this;

            var collection = DbSet.GetCollection();
            if (!collection.IndexExists(keyNames))
            {
                IndexKeysBuilder keys;
                if (isAscending)
                {
                    keys = IndexKeys.Ascending(keyNames);
                }
                else
                {
                    keys = IndexKeys.Descending(keyNames);
                }
                if (isUnique)
                {
                    collection.CreateIndex(keys, IndexOptions.SetUnique(true));
                }
                else
                {
                    collection.CreateIndex(keys);
                }
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
            if (!collection.IndexExists(key))
            {
                collection.CreateIndex(IndexKeys.GeoSpatial(key));
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
            if (!collection.IndexExists(key))
            {
                collection.CreateIndex(IndexKeys.GeoSpatialSpherical(key));
            }
            return this;
        }
    }
}
