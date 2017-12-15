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
        private Type SourceType;

        private Type TargetType;

        private IList<PropertyInfo> Properties;

        private static Dictionary<MapType, IMap> Maps;

        private static object Locker;

        static MapStructPropertyCreator()
        {
            Maps = new Dictionary<MapType, IMap>();
            Locker = new object();
        }

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
            var key = new MapType(SourceType, TargetType);
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
