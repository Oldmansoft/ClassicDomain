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
    public abstract class Context : UnitOfWorkManagedContext, IContext, IDisposable
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
            var dbSet = new DbSet<TDomain, TKey>(GetConfig().GetDatabase(), Commands, keyExpression);
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
            if (!(DbSet[type] is IDbSet<TDomain, TKey> result))
            {
                throw new ClassicDomainException(type, "Set 获取的主键类型与 Add 添加的主键类型不一致。");
            }
            return result;
        }

        /// <summary>
        /// 清空本次工作单元的标识映射。连接由配置缓存持有，这里不关闭客户端。
        /// </summary>
        public void Dispose()
        {
            foreach (var item in DbSet.Values)
            {
                if (item is IClearIdentityMap map) map.ClearIdentityMap();
            }
        }
    }
}
