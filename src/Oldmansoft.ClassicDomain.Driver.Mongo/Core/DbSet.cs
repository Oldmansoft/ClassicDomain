﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Oldmansoft.ClassicDomain.Util;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Core
{
    internal abstract class DbSet<TDomain, TKey> : IDbSet<TDomain, TKey>
    {
        private MongoDatabase Database;

        private MongoCollection<TDomain> Collection;

        /// <summary>
        /// 表名
        /// </summary>
        protected string TableName { get; private set; }
        
        protected ConcurrentQueue<ICommand> Commands { get; private set; }

        /// <summary>
        /// 主键表达式
        /// </summary>
        protected System.Linq.Expressions.Expression<Func<TDomain, TKey>> KeyExpression { get; private set; }

        /// <summary>
        /// 主键获取
        /// </summary>
        protected Func<TDomain, TKey> KeyExpressionCompile { get; private set; }

        /// <summary>
        /// 属性设值器
        /// </summary>
        private ISetter PropertySetter { get; set; }

        /// <summary>
        /// 创建实体集
        /// </summary>
        /// <param name="database"></param>
        /// <param name="commands"></param>
        /// <param name="keyExpression"></param>
        public DbSet(MongoDatabase database, ConcurrentQueue<ICommand> commands, System.Linq.Expressions.Expression<Func<TDomain, TKey>> keyExpression)
        {
            Database = database;
            TableName = typeof(TDomain).Name;
            Commands = commands;
            KeyExpression = keyExpression;
            KeyExpressionCompile = keyExpression.Compile();
            if (typeof(TKey) == typeof(Guid))
            {
                PropertySetter = new PropertySetter<TDomain, TKey>(KeyExpression.GetProperty());
            }
        }

        /// <summary>
        /// 尝试设置主键
        /// </summary>
        /// <param name="domain"></param>
        protected void TrySetDomainKey(TDomain domain)
        {
            if (PropertySetter != null)
            {
                if ((Guid)(object)KeyExpressionCompile(domain) == Guid.Empty)
                {
                    PropertySetter.Set(domain, GuidGenerator.Default.Create(StorageMapping.MongoMapping));
                }
            }
        }

        /// <summary>
        /// 注册添加
        /// </summary>
        /// <param name="domain"></param>
        public abstract void RegisterAdd(TDomain domain);

        /// <summary>
        /// 注册替换
        /// </summary>
        /// <param name="domain"></param>
        public abstract void RegisterReplace(TDomain domain);

        /// <summary>
        /// 注册移除
        /// </summary>
        /// <param name="domain"></param>
        public abstract void RegisterRemove(TDomain domain);

        /// <summary>
        /// 注册执行
        /// </summary>
        /// <param name="execute"></param>
        void IDbSet<TDomain, TKey>.RegisterExecute(Func<MongoCollection<TDomain>, bool> execute)
        {
            Commands.Enqueue(new Commands.ActionCommand<TDomain>(GetCollection(), execute));
        }

        /// <summary>
        /// 获取 Mongo 集
        /// </summary>
        /// <returns></returns>
        public MongoCollection<TDomain> GetCollection()
        {
            if (Collection == null)
            {
                Collection = Database.GetCollection<TDomain>(TableName);
            }
            return Collection;
        }

        /// <summary>
        /// 获取表名
        /// </summary>
        /// <returns></returns>
        string IDbSet<TDomain, TKey>.GetTableName()
        {
            return TableName;
        }

        /// <summary>
        /// 设置表名
        /// </summary>
        /// <param name="tableName"></param>
        void IDbSet<TDomain, TKey>.SetTableName(string tableName)
        {
            TableName = tableName;
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
            int result = 0;
            ICommand command;
            while(Commands.TryDequeue(out command))
            {
                if (command.Execute()) result++;
            }
            return result;
        }
    }
}
