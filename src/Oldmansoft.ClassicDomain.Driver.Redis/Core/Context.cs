using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
using System.Collections.Concurrent;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Core
{
    /// <summary>
    /// 实体上下文
    /// </summary>
    public abstract class Context : UnitOfWorkManagedItem, IContext
    {
        /// <summary>
        /// 配置
        /// </summary>
        private static Config ServerConfig { get; set; }

        static Context()
        {
            ServerConfig = new Config();
        }

        private Dictionary<Type, IDbSet> DbSet { get; set; }

        private ConfigItem Config { get; set; }

        private IDatabase Db { get; set; }

        /// <summary>
        /// 创建实体上下文
        /// </summary>
        public Context()
        {
            DbSet = new Dictionary<Type, IDbSet>();
        }

        private ConfigItem GetConfig()
        {
            if (Config == null) Config = ServerConfig.Get(GetType(), ConnectionName);
            return Config;
        }

        private IDatabase GetDatabase()
        {
            if (Db == null) Db = GetConfig().GetDatabase();
            return Db;
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
            if (DbSet.ContainsKey(type))
            {
                throw new ArgumentException("已添加了具有相同键的项。");
            }

            var dbSet = CreateDbSet(GetConfig(), GetDatabase(), Commands, keyExpression);
            DbSet.Add(type, dbSet);
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
            if (!DbSet.ContainsKey(type))
            {
                throw new ClassicDomainException(type, string.Format("{0} 类型没有添加到 {1} 上下文中。", type.FullName, GetType().FullName));
            }
            var result = DbSet[type] as IDbSet<TDomain, TKey>;
            if (result == null)
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
        internal abstract IDbSet CreateDbSet<TDomain, TKey>(ConfigItem config, IDatabase db, ConcurrentQueue<ICommand> commands, System.Linq.Expressions.Expression<Func<TDomain, TKey>> keyExpression) where TDomain : class, new();
    }
}
