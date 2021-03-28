using System;
using System.Collections.Concurrent;

namespace Oldmansoft.ClassicDomain.Driver
{
    /// <summary>
    /// 标识映射
    /// </summary>
    /// <typeparam name="TDomain"></typeparam>
    public class IdentityMap<TDomain>
    {
        private readonly ConcurrentDictionary<object, TDomain> Store;

        /// <summary>
        /// 获取主键
        /// </summary>
        private Func<TDomain, object> GetKey { get; set; }

        /// <summary>
        /// 创建标识映射
        /// </summary>
        public IdentityMap()
        {
            Store = new ConcurrentDictionary<object, TDomain>();
        }

        /// <summary>
        /// 设置获取主键的表达式
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keyExpression"></param>
        public void SetKey<TKey>(Func<TDomain, TKey> keyExpression)
        {
            GetKey = (entity) => { return keyExpression(entity); };
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="domain"></param>
        public void Set(TDomain domain)
        {
            if (domain == null) return;
            var key = GetKey(domain);
            if (key == null) throw new ArgumentNullException("key");

            var storeDomain = ObjectCreator.CreateInstance<TDomain>();
            Store[key] = Util.DataMapper.Map(domain, storeDomain);
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TDomain Get(object key)
        {
            if (key == null) throw new ArgumentNullException("key");

            if (Store.TryGetValue(key, out TDomain domain)) return domain;
            return default;
        }

        /// <summary>
        /// 移除值
        /// </summary>
        /// <param name="key"></param>
        public void Remove(object key)
        {
            if (key == null) throw new ArgumentNullException("key");
            Store.TryRemove(key, out _);
        }
    }
}
