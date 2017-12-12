using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    class MapStructPropertyCreator
    {
        private static System.Collections.Concurrent.ConcurrentDictionary<long, IMap> Instances;

        static MapStructPropertyCreator()
        {
            Instances = new System.Collections.Concurrent.ConcurrentDictionary<long, IMap>();
        }

        private Type SourceType;

        private Type TargetType;

        private IList<PropertyInfo> Properties;

        public MapStructPropertyCreator(Type sourceType, Type targetType)
        {
            Properties = new List<PropertyInfo>();
            SourceType = sourceType;
            TargetType = targetType;
        }

        public void Add(PropertyInfo property)
        {
            Properties.Add(property);
        }

        public bool HasValue()
        {
            return Properties.Count > 0;
        }

        public IMap CreateMap()
        {
            var key = (long)SourceType.GetHashCode() * int.MaxValue + TargetType.GetHashCode();
            IMap instance;
            if (!Instances.TryGetValue(key, out instance))
            {
                var type = MapTypeBuilder.CreateType(SourceType, TargetType, Properties.ToArray());
                instance = (IMap)Activator.CreateInstance(type);
                Instances.TryAdd(key, instance);
            }
            return instance;
        }
    }
}
