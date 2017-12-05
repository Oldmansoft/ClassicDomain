using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    class MapNullableEnumProperty : MapContentProperty
    {
        public override void Map<TTarget>(string higherName, object source, ref TTarget target, MapConfig config)
        {
            var sourceValue = SourceProperty.GetValue(source);
            if (sourceValue == null && config.IgnoreSourceNull) return;
            if (sourceValue == null)
            {
                TargetProperty.SetValue(target, null);
                return;
            }
            var targetValue = Enum.ToObject(TargetType.GenericTypeArguments[0], (int)sourceValue);
            TargetProperty.SetValue(target, Activator.CreateInstance(TargetType, targetValue));
        }
    }
}
