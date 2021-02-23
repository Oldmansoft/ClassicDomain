using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;

namespace Oldmansoft.ClassicDomain.Driver.InProcess
{
    /// <summary>
    /// 存储管理
    /// </summary>
    /// <typeparam name="TDomain"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    class StoreManager<TDomain, TKey>
    {
        private readonly ConcurrentDictionary<TKey, TDomain> Store;

        private Func<TDomain, TKey> GetKey;

        private Expression<Func<TDomain, TKey>> _KeyExpression;

        /// <summary>
        /// 主键表达式
        /// </summary>
        internal Expression<Func<TDomain, TKey>> KeyExpression
        {
            get { return _KeyExpression; }
            set
            {
                GetKey = value.Compile();
                _KeyExpression = value;
            }
        }

        /// <summary>
        /// 创建存储管理
        /// </summary>
        public StoreManager()
        {
            Store = new ConcurrentDictionary<TKey, TDomain>();
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public bool Add(TDomain domain)
        {
            var key = GetKey(domain);
            if (Store.TryAdd(key, domain))
            {
                return true;
            }
            throw new UniqueException(typeof(TDomain), string.Format("已经存在键 {0}", key));
        }

        /// <summary>
        /// 替换
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public bool Replace(TDomain domain)
        {
            var key = GetKey(domain);
            var oldDomain = Get(key);
            if (oldDomain == null) return false;
            Store.TryUpdate(key, domain, oldDomain);
            return true;
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public bool Remove(TDomain domain)
        {
            var key = GetKey(domain);
            return Store.TryRemove(key, out _);
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TDomain Get(TKey id)
        {
            if (Store.TryGetValue(id, out TDomain result))
            {
                return result;
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        public IQueryable<TDomain> Query()
        {
            return Store.Values.AsQueryable();
        }
    }
}
