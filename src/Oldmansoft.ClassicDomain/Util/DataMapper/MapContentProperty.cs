using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    abstract class MapContentProperty : IMapContent
    {
        protected string PropertyName { get; private set; }

        protected Type SourceType { get; private set; }

        protected Type TargetType { get; private set; }

        protected PropertyInfo SourceProperty { get; private set; }

        protected PropertyInfo TargetProperty { get; private set; }

        public IMapContent Init(string propertyName, Type sourceType, Type targetType, PropertyInfo sourceProperty, PropertyInfo targetProperty)
        {
            PropertyName = propertyName;
            SourceType = sourceType;
            TargetType = targetType;
            SourceProperty = sourceProperty;
            TargetProperty = targetProperty;
            return this;
        }

        public abstract void Map(string higherName, object source, ref object target, MapConfig config);
    }
}
