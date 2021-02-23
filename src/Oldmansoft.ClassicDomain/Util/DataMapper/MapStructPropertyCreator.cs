using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Oldmansoft.ClassicDomain.Util
{
    class MapStructPropertyCreator
    {
        private static readonly Dictionary<long, IMap> Maps;

        private static readonly object Locker;

        static MapStructPropertyCreator()
        {
            Maps = new Dictionary<long, IMap>();
            Locker = new object();
        }

        private readonly Type SourceType;

        private readonly Type TargetType;

        private readonly IList<PropertyInfo> Properties;

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
            IMap map;
            if (Maps.TryGetValue(key, out map)) return map;
            lock (Locker)
            {
                if (Maps.TryGetValue(key, out map)) return map;

                var type = MapTypeBuilder.CreateType(SourceType, TargetType, Properties.ToArray());
                map = (IMap)Activator.CreateInstance(type);
                Maps.Add(key, map);
            }
            return map;
        }
    }
}
