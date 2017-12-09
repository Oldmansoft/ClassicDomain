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
        private Type SourceValueType;

        private Type TargetKeyType;

        private Type TargetValueType;

        private bool IsNormalClass;

        public override IMap Init(Type sourceType, Type targetType, PropertyInfo sourceProperty, PropertyInfo targetProperty)
        {
            base.Init(sourceType, targetType, sourceProperty, targetProperty);
            SourceValueType = SourcePropertyType.GetGenericArguments()[1];
            TargetKeyType = TargetPropertyType.GetGenericArguments()[0];
            TargetValueType = TargetPropertyType.GetGenericArguments()[1];
            IsNormalClass = SourceValueType.IsNormalClass() && TargetValueType.IsNormalClass();
            return this;
        }

        public override void Map(object source, ref object target)
        {
            var sourceValue = Getter.Get(source);
            var currentSource = sourceValue as IDictionary;
            if (currentSource == null)
            {
                Setter.Set(target, null);
                return;
            }

            var targetType = typeof(Dictionary<,>).MakeGenericType(TargetKeyType, TargetValueType);
            var targetValue = Activator.CreateInstance(targetType) as IDictionary;
            foreach (var key in currentSource.Keys)
            {
                targetValue.Add(key, DataMapper.ItemValueCopy(SourceValueType, TargetValueType, IsNormalClass, currentSource[key]));
            }
            Setter.Set(target, targetValue);
        }
    }
}
