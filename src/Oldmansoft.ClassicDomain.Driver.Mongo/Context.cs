using Oldmansoft.ClassicDomain.Driver.Mongo.Core;
using Oldmansoft.ClassicDomain.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Oldmansoft.ClassicDomain.Driver.Mongo
{
    /// <summary>
    /// 实体上下文
    /// 数据库连接串格式 mongodb://[username:password@]host1[:port1][,host2[:port2],...[,hostN[:portN]]][/[database][?options]]
    /// 更多请参考 https://docs.mongodb.com/manual/reference/connection-string/
    /// </summary>
    public abstract class Context : UnitOfWorkManagedContext, IContext
    {
        private static readonly ConfigStore ConfigStore;

        static Context()
        {
            ConfigStore = new ConfigStore();
        }

        private Config Config { get; set; }

        private readonly Dictionary<Type, IDbSet> DbSet;

        /// <summary>
        /// 创建 Mongo 的实体上下文
        /// </summary>
        public Context()
        {
            DbSet = new Dictionary<Type, IDbSet>();
        }

        private Config GetConfig()
        {
            if (Config == null) Config = ConfigStore.Get(GetType());
            return Config;
        }

        /// <summary>
        /// 创建实体集
        /// </summary>
        /// <typeparam name="TDomain"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="commands"></param>
        /// <param name="keyExpression"></param>
        /// <returns></returns>
        internal IDbSet<TDomain, TKey> CreateDbSet<TDomain, TKey>(ConcurrentQueue<ICommand> commands, System.Linq.Expressions.Expression<Func<TDomain, TKey>> keyExpression)
        {
            var database = GetConfig().GetDatabase() as Library.MongoDatabase;
            var result = new DbSet<TDomain, TKey>(database, commands, keyExpression);
            database.SetIdentityMap(result.IdentityMap);
            return result;
        }

        /// <summary>
        /// 添加领域上下文
        /// 主键表达式必须为 Id
        /// </summary>
        /// <typeparam name="TDomain">实体类型</typeparam>
        /// <typeparam name="TKey">主键类型</typeparam>
        /// <param name="keyExpression">主键表达式</param>
        /// <returns>设置</returns>
        public Setting<TDomain, TKey> Add<TDomain, TKey>(System.Linq.Expressions.Expression<Func<TDomain, TKey>> keyExpression)
        {
            Type type = typeof(TDomain);
            if (DbSet.ContainsKey(type))
            {
                throw new ClassicDomainException(type, string.Format("重复添加实体类型 {0}。", type.FullName));
            }
            if (keyExpression == null)
            {
                throw new ArgumentNullException("keyExpression");
            }
            if (keyExpression.GetProperty().Name != "Id")
            {
                throw new ClassicDomainException(type, string.Format("{0} 的主键表达式必须为 Id", type.FullName));
            }
            var dbSet = CreateDbSet(Commands, keyExpression);
            DbSet.Add(type, dbSet);
            return new Setting<TDomain, TKey>(dbSet);
        }

        IDbSet<TDomain, TKey> IContext.Set<TDomain, TKey>()
        {
            Type type = typeof(TDomain);
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
    }
}
