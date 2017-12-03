﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver
{
    /// <summary>
    /// 标识映射
    /// </summary>
    /// <typeparam name="TDomain"></typeparam>
    public class IdentityMap<TDomain>
    {
        private Dictionary<string, TDomain> Store;
        
        /// <summary>
        /// 获取主键
        /// </summary>
        private Func<TDomain, object> GetKey { get; set; }

        /// <summary>
        /// 创建标识映射
        /// </summary>
        public IdentityMap()
        {
            Store = new Dictionary<string, TDomain>();
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
            var storeDomain = Util.ObjectCreator.CreateInstance<TDomain>();
            Store[GetKey(domain).ToString()] = Util.DataMapper.Map(domain, storeDomain);
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TDomain Get(object key)
        {
            if (key == null) throw new ArgumentNullException("key");

            var index = key.ToString();
            if (Store.ContainsKey(index))
            {
                return Store[index];
            }
            else
            {
                return default(TDomain);
            }
        }

        /// <summary>
        /// 移除值
        /// </summary>
        /// <param name="key"></param>
        public void Remove(object key)
        {
            Store.Remove(key.ToString());
        }
    }
}
