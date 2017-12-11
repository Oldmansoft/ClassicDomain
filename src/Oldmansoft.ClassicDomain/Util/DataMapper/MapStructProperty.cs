using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    class MapStructProperty : IMap
    {
        private IList<ISetter> Setters;

        private IList<IGetter> Getters;

        private Type SourceType;

        private Type TargetType;

        public MapStructProperty(Type sourceType, Type targetType)
        {
            Setters = new List<ISetter>();
            Getters = new List<IGetter>();
            SourceType = sourceType;
            TargetType = targetType;
        }

        public void Add(PropertyInfo sourceProperty, PropertyInfo targetProperty)
        {
            Getters.Add((IGetter)Activator.CreateInstance(typeof(GetterWrapper<,>).MakeGenericType(SourceType, sourceProperty.PropertyType), sourceProperty));
            Setters.Add((ISetter)Activator.CreateInstance(typeof(SetterWrapper<,>).MakeGenericType(TargetType, targetProperty.PropertyType), targetProperty));
        }

        public bool HasValue()
        {
            return Setters.Count > 0;
        }

        public void Map(object source, ref object target)
        {
            for (var i = 0; i < Setters.Count; i++)
            {
                Setters[i].Set(target, Getters[i].Get(source));
            }
        }
    }
}
