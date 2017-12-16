using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    class MapDictionaryProperty : MapProperty
    {
        private Type SourcePropertyValueType;

        private Type TargetPropertyKeyType;

        private Type TargetPropertyValueType;

        private bool IsNormalClass;

        public override IMap Init(Type sourceType, Type targetType, PropertyInfo sourceProperty, PropertyInfo targetProperty)
        {
            base.Init(sourceType, targetType, sourceProperty, targetProperty);
            SourcePropertyValueType = SourcePropertyType.GetGenericArguments()[1];
            TargetPropertyKeyType = TargetPropertyType.GetGenericArguments()[0];
            TargetPropertyValueType = TargetPropertyType.GetGenericArguments()[1];
            IsNormalClass = SourcePropertyValueType.IsNormalClass() && TargetPropertyValueType.IsNormalClass();
            return this;
        }

        public override void Map(object source, object target)
        {
            var sourceValue = Getter.Get(source) as IDictionary;
            if (sourceValue == null)
            {
                Setter.Set(target, null);
                return;
            }

            var targetValue = ObjectCreator.CreateInstance(TargetPropertyType) as IDictionary;
            foreach (var key in sourceValue.Keys)
            {
                targetValue.Add(key, DataMapper.ItemValueCopy(SourcePropertyValueType, TargetPropertyValueType, IsNormalClass, sourceValue[key]));
            }
            Setter.Set(target, targetValue);
        }
    }
}
