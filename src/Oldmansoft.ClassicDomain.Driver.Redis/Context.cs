using Oldmansoft.ClassicDomain.Driver.Redis.Core;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Oldmansoft.ClassicDomain.Driver.Redis
{
    /// <summary>
    /// 实体上下文
    /// </summary>
    public abstract class Context : UnitOfWorkManagedContext, IContext
    {
        /// <summary>
        /// 配置
        /// </summary>
        private static ConfigStore ConfigStore { get; set; }

        static Context()
        {
            ConfigStore = new ConfigStore();
        }

        private readonly Dictionary<Type, IDbSet> DbSetStore;

        private Config Config { get; set; }

        private IDatabase Database { get; set; }

        /// <summary>
        /// 是否低服务器版本
        /// </summary>
        public bool IsLowServerVersion { get; set; }

        /// <summary>
        /// 创建实体上下文
        /// </summary>
        public Context()
        {
            DbSetStore = new Dictionary<Type, IDbSet>();
        }

        private Config GetConfig()
        {
            if (Config == null) Config = ConfigStore.Get(GetType(), IsLowServerVersion);
            return Config;
        }

        private IDatabase GetDatabase()
        {
            if (Database == null) Database = GetConfig().GetDatabase();
            return Database;
        }

        /// <summary>
        /// 添加领域上下文
        /// </summary>
        /// <typeparam name="TDomain"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keyExpression"></param>
        public void Add<TDomain, TKey>(System.Linq.Expressions.Expression<Func<TDomain, TKey>> keyExpression) where TDomain : class, new()
        {
            var type = typeof(TDomain);
            if (DbSetStore.ContainsKey(type))
            {
                throw new ClassicDomainException(type, string.Format("重复添加实体类型 {0}。", type.FullName));
            }

            var dbSet = CreateDbSet(GetConfig(), GetDatabase(), Commands, keyExpression);
            DbSetStore.Add(type, dbSet);
        }

        /// <summary>
        /// 获取实体集
        /// </summary>
        /// <typeparam name="TDomain"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        IDbSet<TDomain, TKey> IContext.Set<TDomain, TKey>()
        {
            var type = typeof(TDomain);
            if (!DbSetStore.ContainsKey(type))
            {
                throw new ClassicDomainException(type, string.Format("{0} 类型没有添加到 {1} 上下文中。", type.FullName, GetType().FullName));
            }
            if (!(DbSetStore[type] is IDbSet<TDomain, TKey> result))
            {
                throw new ClassicDomainException(type, "Set 获取的主键类型与 Add 添加的主键类型不一致。");
            }
            return result;
        }

        /// <summary>
        /// 创建实体集
        /// </summary>
        /// <typeparam name="TDomain"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="config"></param>
        /// <param name="db"></param>
        /// <param name="commands"></param>
        /// <param name="keyExpression"></param>
        /// <returns></returns>
        internal abstract IDbSet CreateDbSet<TDomain, TKey>(Config config, IDatabase db, ConcurrentQueue<ICommand> commands, System.Linq.Expressions.Expression<Func<TDomain, TKey>> keyExpression) where TDomain : class, new();
    }
}
