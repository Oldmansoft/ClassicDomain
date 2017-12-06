using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    class MapListProperty : MapContentProperty
    {
        public override void Map(object source, ref object target)
        {
            var sourceValue = SourceProperty.GetValue(source);
            if (sourceValue == null)
            {
                TargetProperty.SetValue(target, null);
                return;
            }

            var sourceItemType = SourceType.GetGenericArguments()[0];
            var targetItemType = TargetType.GetGenericArguments()[0];
            var targetType = typeof(List<>).MakeGenericType(targetItemType);

            var currentSource = (sourceValue as IEnumerable);
            if (currentSource == null)
            {
                TargetProperty.SetValue(target, null);
                return;
            }

            var isNormalClass = sourceItemType.IsNormalClass() && targetItemType.IsNormalClass();
            var targetValue = ObjectCreator.CreateInstance(targetType) as IList;
            foreach (var item in currentSource)
            {
                targetValue.Add(DataMapper.ItemValueCopy(sourceItemType, targetItemType, isNormalClass, item));
            }
            TargetProperty.SetValue(target, targetValue);
        }
    }
}
