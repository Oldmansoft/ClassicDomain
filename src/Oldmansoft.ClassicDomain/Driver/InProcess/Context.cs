using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Oldmansoft.ClassicDomain.Driver.InProcess
{
    /// <summary>
    /// 进程内上下文
    /// </summary>
    public abstract class Context : UnitOfWorkManagedContext
    {
        private ConcurrentDictionary<Type, IDbSet> DbSet { get; set; }

        /// <summary>
        /// 创建进程上下文
        /// </summary>
        public Context()
        {
            DbSet = new ConcurrentDictionary<Type, IDbSet>();
        }

        /// <summary>
        /// 添加领域上下文
        /// </summary>
        /// <typeparam name="TDomain">实体类型</typeparam>
        /// <typeparam name="TKey">主键类型</typeparam>
        /// <param name="keyExpression">主键表达式</param>
        public void Add<TDomain, TKey>(System.Linq.Expressions.Expression<Func<TDomain, TKey>> keyExpression)
        {
            if (!DbSet.TryAdd(typeof(TDomain), new DbSet<TDomain, TKey>(Commands, keyExpression)))
            {
                throw new ArgumentException("已添加了具有相同键的项。");
            }
        }

        /// <summary>
        /// 获取进程实体集
        /// </summary>
        /// <typeparam name="TDomain"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        internal DbSet<TDomain, TKey> Set<TDomain, TKey>()
        {
            IDbSet result;
            if (!DbSet.TryGetValue(typeof(TDomain), out result))
            {
                throw new KeyNotFoundException("给定关键字不在字典中。");
            }
            return result as DbSet<TDomain, TKey>;
        }
    }
}
