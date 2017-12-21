using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    class MapNormalClassProperty : MapProperty
    {
        private IGetter TargetGetter { get; set; }

        public override IMap Init(Type sourceType, Type targetType, PropertyInfo sourceProperty, PropertyInfo targetProperty)
        {
            TargetGetter = (IGetter)Activator.CreateInstance(typeof(PropertyGetter<,>).MakeGenericType(targetType, targetProperty.PropertyType), targetProperty);
            return base.Init(sourceType, targetType, sourceProperty, targetProperty);
        }

        public override void Map(object source, object target)
        {
            var sourceValue = Getter.Get(source);
            if (sourceValue == null)
            {
                Setter.Set(target, null);
                return;
            }
            
            var targetValue = TargetGetter.Get(target);
            if (targetValue == null)
            {
                targetValue = ObjectCreator.CreateInstance(TargetPropertyType);
                if (targetValue == null) return;
            }
            DataMapper.NormalClassCopy(sourceValue, SourcePropertyType, ref targetValue, TargetPropertyType);
            Setter.Set(target, targetValue);
        }
    }
}
