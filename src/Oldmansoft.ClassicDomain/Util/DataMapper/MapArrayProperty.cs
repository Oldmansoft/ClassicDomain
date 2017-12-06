using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    class MapArrayProperty : MapContentProperty
    {
        public override void Map(object source, ref object target)
        {
            var sourceValue = SourceProperty.GetValue(source);
            if (sourceValue == null)
            {
                TargetProperty.SetValue(target, null);
                return;
            }

            var sourceItemType = SourceType.GetMethod("Set").GetParameters()[1].ParameterType;
            var targetItemType = TargetType.GetMethod("Set").GetParameters()[1].ParameterType;

            var isNormalClass = sourceItemType.IsNormalClass() && targetItemType.IsNormalClass();
            var currentSource = sourceValue as Array;
            if (currentSource == null)
            {
                TargetProperty.SetValue(target, null);
                return;
            }

            var targetValue = TargetType.InvokeMember("Set", BindingFlags.CreateInstance, null, null, new object[] { currentSource.Length });
            var method = TargetType.GetMethod("SetValue", new Type[] { typeof(object), typeof(int) });
            int index = 0;
            foreach (var item in currentSource)
            {
                method.Invoke(targetValue, new object[] { DataMapper.ItemValueCopy(sourceItemType, targetItemType, isNormalClass, item), index });
                index++;
            }
            TargetProperty.SetValue(target, targetValue);
        }
    }
}
