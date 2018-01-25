using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.InProcess
{
    /// <summary>
    /// 存储管理
    /// </summary>
    /// <typeparam name="TDomain"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    class StoreManager<TDomain, TKey>
    {
        private ConcurrentDictionary<TKey, TDomain> Store { get; set; }

        private Func<TDomain, TKey> GetKey { get; set; }

        private System.Linq.Expressions.Expression<Func<TDomain, TKey>> _KeyExpression { get; set; }

        /// <summary>
        /// 主键表达式
        /// </summary>
        internal System.Linq.Expressions.Expression<Func<TDomain, TKey>> KeyExpression
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
            TDomain temp;
            return Store.TryRemove(key, out temp);
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TDomain Get(TKey id)
        {
            TDomain result;
            if (Store.TryGetValue(id, out result))
            {
                return result;
            }
            else
            {
                return default(TDomain);
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
