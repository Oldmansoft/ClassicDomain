using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    class MapNormalClassProperty : MapContentProperty
    {
        public override void Map(object source, ref object target)
        {
            object sourceValue = SourceProperty.GetValue(source);
            if (sourceValue == null)
            {
                TargetProperty.SetValue(target, null);
                return;
            }

            if (sourceValue == null)
            {
                TargetProperty.SetValue(target, null);
                return;
            }
            object targetValue = TargetProperty.GetValue(target);
            if (targetValue == null)
            {
                targetValue = ObjectCreator.CreateInstance(TargetType);
                if (targetValue == null) return;
            }
            TargetProperty.SetValue(target, targetValue);
            DataMapper.CopyNormal(sourceValue, SourceType, ref targetValue, TargetType);
        }
    }
}
