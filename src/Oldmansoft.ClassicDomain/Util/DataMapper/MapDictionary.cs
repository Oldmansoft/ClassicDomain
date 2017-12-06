using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    class MapDictionary : MapContent
    {
        public override void Map(object source, ref object target)
        {
            if (source == null)
            {
                target = null;
                return;
            }

            var currentSource = source as IDictionary;
            if (currentSource == null)
            {
                target = null;
                return;
            }

            var sourceKeyType = SourceType.GetGenericArguments()[0];
            var sourceValueType = SourceType.GetGenericArguments()[1];
            var targetKeyType = TargetType.GetGenericArguments()[0];
            var targetValueType = TargetType.GetGenericArguments()[1];

            var targetType = typeof(Dictionary<,>).MakeGenericType(targetKeyType, targetValueType);
            var isNormalClass = sourceValueType.IsNormalClass() && targetValueType.IsNormalClass();
            var targetValue = target as IDictionary;
            foreach (var key in currentSource.Keys)
            {
                targetValue.Add(key, DataMapper.ItemValueCopy(sourceValueType, targetValueType, isNormalClass, currentSource[key]));
            }
        }
    }
}
