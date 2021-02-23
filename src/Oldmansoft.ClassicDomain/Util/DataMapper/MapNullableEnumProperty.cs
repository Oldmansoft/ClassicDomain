using System;

namespace Oldmansoft.ClassicDomain.Util
{
    class MapNullableEnumProperty : MapProperty
    {
        public override void Map(object source, object target)
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
