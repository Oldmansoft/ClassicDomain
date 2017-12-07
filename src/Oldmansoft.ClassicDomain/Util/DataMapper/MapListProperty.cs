using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    class MapListProperty : MapProperty
    {
        public override void Map(object source, ref object target)
        {
            var sourceValue = Getter.Get(source);
            if (sourceValue == null)
            {
                Setter.Set(target, null);
                return;
            }

            var sourceItemType = SourcePropertyType.GetGenericArguments()[0];
            var targetItemType = TargetPropertyType.GetGenericArguments()[0];
            var targetType = typeof(List<>).MakeGenericType(targetItemType);

            var currentSource = (sourceValue as IEnumerable);
            if (currentSource == null)
            {
                Setter.Set(target, null);
                return;
            }

            var isNormalClass = sourceItemType.IsNormalClass() && targetItemType.IsNormalClass();
            var targetValue = Activator.CreateInstance(targetType) as IList;
            foreach (var item in currentSource)
            {
                targetValue.Add(DataMapper.ItemValueCopy(sourceItemType, targetItemType, isNormalClass, item));
            }
            Setter.Set(target, targetValue);
        }
    }
}
