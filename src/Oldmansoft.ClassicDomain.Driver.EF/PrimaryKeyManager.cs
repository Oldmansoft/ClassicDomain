using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Reflection;
using System.Data.Entity.Infrastructure;
using Oldmansoft.ClassicDomain.Util;

namespace Oldmansoft.ClassicDomain.Driver.EF
{
    class PrimaryKeyManager
    {
        public static readonly PrimaryKeyManager Instance = new PrimaryKeyManager();

        private ConcurrentDictionary<Type, IPropertyValue> GuidPrimaryKeySet { get; set; }

        private PrimaryKeyManager()
        {
            GuidPrimaryKeySet = new ConcurrentDictionary<Type, IPropertyValue>();
        }

        public void TrySetDomainGuidEmptyKey<TDomain>(TDomain domain, Context context) where TDomain : class
        {
            var property = GetPrimaryKeyGuidProperty<TDomain>(context) as PropertyWrapper<TDomain, Guid>;
            if (property == null) return;
            if (property.Get(domain) != Guid.Empty) return;

            property.Set(domain, GuidGenerator.Default.Create(StorageMapping.MssqlMapping));
        }

        private IPropertyValue GetPrimaryKeyGuidProperty<TDomain>(Context context) where TDomain : class
        {
            IPropertyValue result;
            if (GuidPrimaryKeySet.TryGetValue(typeof(TDomain), out result)) return result;

            return SetPrimaryKeyGuidProperty<TDomain>(context);
        }

        private IPropertyValue SetPrimaryKeyGuidProperty<TDomain>(Context context) where TDomain : class
        {
            var objectContext = ((IObjectContextAdapter)context).ObjectContext;
            var keyNames = objectContext.CreateObjectSet<TDomain>().EntitySet.ElementType.KeyMembers
                .Select(k => k.Name).ToArray();
            if (keyNames.Length == 0) throw new KeyNotFoundException("实体没有定义主键");
            if (keyNames.Length > 1) throw new ArgumentOutOfRangeException("domain", "实体的主键只能定义一个");
            
            var property = TypePublicInstanceStore.GetPropertys<TDomain>().First(o => o.Name == keyNames[0]);
            PropertyWrapper<TDomain, Guid> result = null;
            if (property.PropertyType == typeof(Guid))
            {
                result = new PropertyWrapper<TDomain, Guid>(property);
            }

            GuidPrimaryKeySet.TryAdd(typeof(TDomain), result);
            return result;
        }
    }
}
