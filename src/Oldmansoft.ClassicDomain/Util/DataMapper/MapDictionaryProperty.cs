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
        public override void Map(object source, ref object target)
        {
            var sourceValue = Getter.Get(source);
            if (sourceValue == null)
            {
                Setter.Set(target, null);
                return;
            }

            var currentSource = sourceValue as IDictionary;
            if (currentSource == null)
            {
                Setter.Set(target, null);
                return;
            }

            var sourceKeyType = SourcePropertyType.GetGenericArguments()[0];
            var sourceValueType = SourcePropertyType.GetGenericArguments()[1];
            var targetKeyType = TargetPropertyType.GetGenericArguments()[0];
            var targetValueType = TargetPropertyType.GetGenericArguments()[1];

            var targetType = typeof(Dictionary<,>).MakeGenericType(targetKeyType, targetValueType);
            var isNormalClass = sourceValueType.IsNormalClass() && targetValueType.IsNormalClass();
            var targetValue = Activator.CreateInstance(targetType) as IDictionary;
            foreach (var key in currentSource.Keys)
            {
                targetValue.Add(key, DataMapper.ItemValueCopy(sourceValueType, targetValueType, isNormalClass, currentSource[key]));
            }
            Setter.Set(target, targetValue);
        }
    }
}
