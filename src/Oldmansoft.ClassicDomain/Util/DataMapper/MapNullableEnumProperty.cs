using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    class MapNullableEnumProperty : MapProperty
    {
        public override void Map(object source, ref object target)
        {
            var sourceValue = Getter.Get(source);
            if (sourceValue == null)
            {
                Setter.Set(target, null);
                return;
            }
            var targetValue = Enum.ToObject(TargetPropertyType.GenericTypeArguments[0], (int)sourceValue);
            Setter.Set(target, Activator.CreateInstance(TargetPropertyType, targetValue));
        }
    }
}
