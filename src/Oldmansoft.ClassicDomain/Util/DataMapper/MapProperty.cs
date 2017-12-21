using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    abstract class MapProperty : IMap
    {
        protected IGetter Getter { get; private set; }

        protected ISetter Setter { get; private set; }

        protected Type SourcePropertyType { get; private set; }

        protected Type TargetPropertyType { get; private set; }
        
        public virtual IMap Init(Type sourceType, Type targetType, PropertyInfo sourceProperty, PropertyInfo targetProperty)
        {
            Getter = (IGetter)Activator.CreateInstance(typeof(PropertyGetter<,>).MakeGenericType(sourceType, sourceProperty.PropertyType), sourceProperty);
            Setter = (ISetter)Activator.CreateInstance(typeof(PropertySetter<,>).MakeGenericType(targetType, targetProperty.PropertyType), targetProperty);
            SourcePropertyType = sourceProperty.PropertyType;
            TargetPropertyType = targetProperty.PropertyType;
            return this;
        }

        public abstract void Map(object source, object target);
    }
}
