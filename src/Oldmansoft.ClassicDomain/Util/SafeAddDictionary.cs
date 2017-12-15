using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    /// <summary>
    /// 线程添加安全字典
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class SafeAddDictionary<TKey, TValue>
    {
        private ConcurrentDictionary<TKey, Lazy<TValue>> Store;

        private Func<TKey, Lazy<TValue>> ValueCreate;
        
        /// <summary>
        /// 创建字典
        /// </summary>
        /// <param name="valueCreate"></param>
        public SafeAddDictionary(Func<TKey, TValue> valueCreate)
        {
            Store = new ConcurrentDictionary<TKey, Lazy<TValue>>();
            ValueCreate = (key) => new Lazy<TValue>(() => valueCreate(key));
        }

        /// <summary>
        /// 获取或添加
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue GetOrAdd(TKey key)
        {
            return Store.GetOrAdd(key, ValueCreate).Value;
        }
        
        /// <summary>
        /// 尝试获取
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            Lazy<TValue> lazy;
            if (Store.TryGetValue(key, out lazy))
            {
                value = lazy.Value;
                return true;
            }
            value = default(TValue);
            return false;
        }

        /// <summary>
        /// 是否包含
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            return Store.ContainsKey(key);
        }
    }
}
