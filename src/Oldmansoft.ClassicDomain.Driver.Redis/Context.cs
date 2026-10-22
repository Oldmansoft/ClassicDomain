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
    public abstract class Context : UnitOfWorkManagedContext, IContext, IDisposable, IUnitOfWorkFlushable, Core.IRedisFlush
    {
        /// <summary>
        /// 配置
        /// </summary>
        private static ConfigStore ConfigStore { get; set; }

        /// <summary>
        /// 是否运行在低版本的服务器上
        /// 低版本的服务器不支持命令 rpush
        /// </summary>
        public static bool IsLowServerVersion { get; set; }

        static Context()
        {
            ConfigStore = new ConfigStore();
        }

        private readonly Dictionary<Type, IDbSet> DbSetStore;

        private readonly List<Library.RedisPendingWrite> Pending = new List<Library.RedisPendingWrite>();

        private int FlushedCount;

        private Config Config;

        private IDatabase Database;

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
        internal IDbSet CreateDbSet<TDomain, TKey>(Config config, IDatabase db, ConcurrentQueue<ICommand> commands, System.Linq.Expressions.Expression<Func<TDomain, TKey>> keyExpression) where TDomain : class, new()
        {
            return new SafeModeDbSet<TDomain, TKey>(config, db, commands, keyExpression, this);
        }

        /// <summary>
        /// 清空本次工作单元的标识映射
        /// </summary>
        public void Dispose()
        {
            Pending.Clear();
            FlushedCount = 0;
            foreach (var item in DbSetStore.Values)
            {
                if (item is IClearIdentityMap map) map.ClearIdentityMap();
            }
        }

        void Core.IRedisFlush.Enqueue(Library.RedisPendingWrite write)
        {
            Pending.Add(write);
        }

        void Core.IRedisFlush.Send()
        {
            SendPending();
        }

        int IUnitOfWorkFlushable.Flush()
        {
            SendPending();
            var count = FlushedCount;
            FlushedCount = 0;
            return count;
        }

        void IUnitOfWorkFlushable.Abandon()
        {
            for (var i = Pending.Count - 1; i >= 0; i--)
            {
                if (Pending[i].OnFailure != null) Pending[i].OnFailure();
            }
            Pending.Clear();
            FlushedCount = 0;
        }

        void SendPending()
        {
            if (Pending.Count == 0) return;
            var batch = GetDatabase().CreateBatch();
            var tasks = new List<System.Threading.Tasks.Task<StackExchange.Redis.RedisResult>>(Pending.Count);
            foreach (var write in Pending)
            {
                tasks.Add(batch.ScriptEvaluateAsync(Library.RedisScript.Source, write.Keys, write.Arguments));
            }
            batch.Execute();
            Exception first = null;
            for (var i = 0; i < tasks.Count; i++)
            {
                try
                {
                    var result = tasks[i].GetAwaiter().GetResult();
                    if (Succeeded(result)) FlushedCount++;
                    else if (Pending[i].OnFailure != null) Pending[i].OnFailure();
                }
                catch (Exception ex)
                {
                    if (Pending[i].OnFailure != null) Pending[i].OnFailure();
                    if (first == null) first = Translate(ex, Pending[i].DomainType);
                }
            }
            Pending.Clear();
            if (first != null) throw first;
        }

        static bool Succeeded(StackExchange.Redis.RedisResult result)
        {
            if (result == null || result.IsNull) return false;
            var value = (StackExchange.Redis.RedisValue)result;
            return !value.IsNull && (long)value == 1;
        }

        static Exception Translate(Exception ex, Type domainType)
        {
            var redis = ex as StackExchange.Redis.RedisServerException ?? ex.InnerException as StackExchange.Redis.RedisServerException;
            if (redis != null && redis.Message.IndexOf("wrong kind of value", StringComparison.Ordinal) >= 0)
            {
                return new ClassicDomainException(domainType, "数据冲突：存在着相同记录的不同类型数据，可能是之前使用过快速模式保存过。");
            }
            if (redis != null && redis.Message == "ERR wrong number of arguments for 'rpush' command")
            {
                return new ClassicDomainException(domainType, Core.ConfigStore.AlertLowServerVersion);
            }
            return ex;
        }
    }
}
