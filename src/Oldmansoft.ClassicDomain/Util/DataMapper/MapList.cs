using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    class MapList : MapContent
    {
        public override void Map(string higherName, object source, ref object target, MapConfig config)
        {
            if (source == null && config.IgnoreSourceNull) return;

            var sourceItemType = SourceType.GetGenericArguments()[0];
            var targetItemType = TargetType.GetGenericArguments()[0];
            var targetType = typeof(List<>).MakeGenericType(targetItemType);

            var currentSource = (source as IEnumerable);
            if (currentSource == null)
            {
                target = null;
                return;
            }

            var isNormalClass = sourceItemType.IsNormalClass() && targetItemType.IsNormalClass();
            var targetValue = target as IList;
            foreach (var item in currentSource)
            {
                targetValue.Add(DataMapper.ItemValueCopy(sourceItemType, targetItemType, isNormalClass, item, config));
            }
        }
    }
}
