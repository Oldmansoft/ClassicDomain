using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Reflection;
using System.Data.Entity.Infrastructure;

namespace Oldmansoft.ClassicDomain.Driver.EF
{
    class PrimaryKeyManager
    {
        public static readonly PrimaryKeyManager Instance = new PrimaryKeyManager();

        private ConcurrentDictionary<Type, PropertyInfo> GuidPrimaryKeySet { get; set; }

        private PrimaryKeyManager()
        {
            GuidPrimaryKeySet = new ConcurrentDictionary<Type, PropertyInfo>();
        }

        public void TrySetDomainGuidEmptyKey<TDomain>(TDomain domain, Context context) where TDomain : class
        {
            var property = GetPrimaryKeyGuidProperty<TDomain>(context);
            if (property == null) return;
            if ((Guid)property.GetValue(domain) != Guid.Empty) return;

            property.SetValue(domain, GuidGenerator.Default.Create(StorageMapping.MssqlMapping));
        }

        private PropertyInfo GetPrimaryKeyGuidProperty<TDomain>(Context context) where TDomain : class
        {
            PropertyInfo result;
            if (GuidPrimaryKeySet.TryGetValue(typeof(TDomain), out result)) return result;

            return SetPrimaryKeyGuidProperty<TDomain>(context);
        }

        private PropertyInfo SetPrimaryKeyGuidProperty<TDomain>(Context context) where TDomain : class
        {
            var objectContext = ((IObjectContextAdapter)context).ObjectContext;
            var keyNames = objectContext.CreateObjectSet<TDomain>().EntitySet.ElementType.KeyMembers
                .Select(k => k.Name).ToArray();
            if (keyNames.Length == 0) throw new KeyNotFoundException("实体没有定义主键");
            if (keyNames.Length > 1) throw new ArgumentOutOfRangeException("domain", "实体的主键只能定义一个");
            var property = Util.TypePublicInstanceStore.GetPropertys<TDomain>().First(o => o.Name == keyNames[0]);
            if (property.PropertyType != typeof(Guid))
            {
                property = null;
            }

            GuidPrimaryKeySet.TryAdd(typeof(TDomain), property);
            return property;
        }
    }
}
