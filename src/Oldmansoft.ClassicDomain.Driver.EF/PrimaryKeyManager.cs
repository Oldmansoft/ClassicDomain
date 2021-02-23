using Oldmansoft.ClassicDomain.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace Oldmansoft.ClassicDomain.Driver.EF
{
    class PrimaryKeyManager
    {
        public static readonly PrimaryKeyManager Instance = new PrimaryKeyManager();

        private ConcurrentDictionary<Type, IValue> GuidPrimaryKeySet { get; set; }

        private PrimaryKeyManager()
        {
            GuidPrimaryKeySet = new ConcurrentDictionary<Type, IValue>();
        }

        /// <summary>
        /// 尝试给 Guid 赋值
        /// </summary>
        /// <typeparam name="TDomain"></typeparam>
        /// <param name="domain"></param>
        /// <param name="context"></param>
        public void TrySetDomainGuidEmptyKey<TDomain>(TDomain domain, Context context) where TDomain : class
        {
            var value = GetPrimaryKey<TDomain>(context) as PropertyWrapper<TDomain, Guid>;
            if (value == null) return;
            if (value.Get(domain) != Guid.Empty) return;

            value.Set(domain, GuidGenerator.Default.Create(StorageMapping.MssqlMapping));
        }

        /// <summary>
        /// 获取主键的值操作接口
        /// </summary>
        /// <typeparam name="TDomain"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public IValue GetPrimaryKey<TDomain>(Context context) where TDomain : class
        {
            IValue result;
            if (GuidPrimaryKeySet.TryGetValue(typeof(TDomain), out result)) return result;

            var objectContext = ((IObjectContextAdapter)context).ObjectContext;
            var keyNames = objectContext.CreateObjectSet<TDomain>().EntitySet.ElementType.KeyMembers
                .Select(k => k.Name).ToArray();
            if (keyNames.Length == 0) throw new KeyNotFoundException("实体没有定义主键");
            if (keyNames.Length > 1) throw new ArgumentOutOfRangeException("domain", "实体的主键只能定义一个");

            result = TypePublicInstancePropertyInfoStore.GetValue<TDomain>(o => o.Name == keyNames[0]);
            GuidPrimaryKeySet.TryAdd(typeof(TDomain), result);
            return result;
        }
    }
}
