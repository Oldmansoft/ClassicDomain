using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    class MapArray : MapContent
    {
        public override void Map(string higherName, object source, ref object target, MapConfig config)
        {
            if (source == null && config.IgnoreSourceNull) return;

            var sourceItemType = SourceType.GetMethod("Set").GetParameters()[1].ParameterType;
            var targetItemType = TargetType.GetMethod("Set").GetParameters()[1].ParameterType;

            var isNormalClass = sourceItemType.IsNormalClass() && targetItemType.IsNormalClass();
            var currentSource = source as Array;
            if (currentSource == null)
            {
                target = null;
                return;
            }

            var targetValue = target as Array;
            var method = TargetType.GetMethod("SetValue", new Type[] { typeof(object), typeof(int) });
            int index = 0;
            foreach (var item in currentSource)
            {
                method.Invoke(targetValue, new object[] { DataMapper.ItemValueCopy(sourceItemType, targetItemType, isNormalClass, item, config), index });
                index++;
            }
        }
    }
}
