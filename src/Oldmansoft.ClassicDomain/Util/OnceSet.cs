using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    /// <summary>
    /// 一次性使用器
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class OnceSet<TKey>
    {
        private Dictionary<TKey, bool> Store { get; set; }

        /// <summary>
        /// 创建一次性使用器
        /// </summary>
        public OnceSet()
        {
            Store = new Dictionary<TKey, bool>();
        }

        /// <summary>
        /// 获取和设置值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool this[TKey key]
        {
            get
            {
                if (!Store.ContainsKey(key))
                {
                    lock (Store)
                    {
                        if (!Store.ContainsKey(key))
                        {
                            Store.Add(key, false);
                        }
                    }
                }
                return Store[key];
            }
            set
            {
                if (!Store.ContainsKey(key))
                {
                    lock (Store)
                    {
                        if (!Store.ContainsKey(key))
                        {
                            Store.Add(key, value);
                            return;
                        }
                    }
                }
                Store[key] = value;
            }
        }

        /// <summary>
        /// 使用
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>是否成功</returns>
        public bool Use(TKey key)
        {
            lock (Store)
            {
                if (!Store.ContainsKey(key))
                {
                    Store.Add(key, true);
                    return true;
                }
                else
                {
                    if (Store[key]) return false;
                    Store[key] = true;
                    return true;
                }
            }
        }
    }
}
