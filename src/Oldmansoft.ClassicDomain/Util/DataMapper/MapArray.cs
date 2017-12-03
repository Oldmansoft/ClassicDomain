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
        public override void Map<TTarget>(string higherName, object source, ref TTarget target, MapConfig config)
        {
            var sourceValue = ValueAction.GetValue(source);
            if (sourceValue == null && config.IgnoreSourceNull) return;

            var sourceItemType = SourceType.GetMethod("Set").GetParameters()[1].ParameterType;
            var targetItemType = TargetType.GetMethod("Set").GetParameters()[1].ParameterType;

            var isNormalClass = sourceItemType.IsNormalClass() && targetItemType.IsNormalClass();
            var currentSource = sourceValue as Array;
            if (currentSource == null)
            {
                ValueAction.SetValue(ref target, null);
                return;
            }

            var targetValue = TargetType.InvokeMember("Set", BindingFlags.CreateInstance, null, null, new object[] { currentSource.Length }) as Array;
            var method = TargetType.GetMethod("SetValue", new Type[] { typeof(object), typeof(int) });
            int index = 0;
            foreach (var item in currentSource)
            {
                method.Invoke(targetValue, new object[] { DataMapper.ItemValueCopy(sourceItemType, targetItemType, isNormalClass, item, config), index });
                index++;
            }
            ValueAction.SetValue(ref target, targetValue);
            return;
        }
    }
}
