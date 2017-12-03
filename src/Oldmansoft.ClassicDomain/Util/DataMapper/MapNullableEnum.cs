using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    class MapNullableEnum : MapContent
    {
        public override void Map<TTarget>(string higherName, object source, ref TTarget target, MapConfig config)
        {
            var sourceValue = ValueAction.GetValue(source);
            if (sourceValue == null && config.IgnoreSourceNull) return;
            if (sourceValue == null)
            {
                ValueAction.SetValue(ref target, null);
                return;
            }
            var targetValue = Enum.ToObject(TargetType.GenericTypeArguments[0], (int)sourceValue);
            ValueAction.SetValue(ref target, Activator.CreateInstance(TargetType, targetValue));
        }
    }
}
