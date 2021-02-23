using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Oldmansoft.ClassicDomain.Util;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Core
{
    internal class DbSet<TDomain, TKey> : IDbSet<TDomain, TKey>
    {
        private readonly MongoDatabase Database;

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
        /// 标识映射
        /// </summary>
        public IdentityMap<TDomain> IdentityMap { get; private set; }

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
            IdentityMap = new IdentityMap<TDomain>();
            IdentityMap.SetKey(keyExpression.Compile());
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
        public void RegisterAdd(TDomain domain)
        {
            TrySetDomainKey(domain);
            domain = domain.MapTo<TDomain>();
            Commands.Enqueue(new Commands.AddCommand<TDomain>(GetCollection(), domain, IdentityMap));
        }

        /// <summary>
        /// 注册替换
        /// </summary>
        /// <param name="domain"></param>
        public void RegisterReplace(TDomain domain)
        {
            domain = domain.MapTo<TDomain>();
            Commands.Enqueue(new Commands.ReplaceCommand<TDomain, TKey>(GetCollection(), KeyExpressionCompile(domain), domain, IdentityMap));
        }

        /// <summary>
        /// 注册移除
        /// </summary>
        /// <param name="domain"></param>
        public void RegisterRemove(TDomain domain)
        {
            domain = domain.MapTo<TDomain>();
            Commands.Enqueue(new Commands.RemoveCommand<TDomain, TKey>(GetCollection(), KeyExpression, KeyExpressionCompile(domain), IdentityMap));
        }

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
    }
}
