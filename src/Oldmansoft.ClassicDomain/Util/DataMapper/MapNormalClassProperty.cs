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
        public override void Map<TTarget>(string higherName, object source, ref TTarget target, MapConfig config)
        {
            object sourceValue = SourceProperty.GetValue(source);
            if (sourceValue == null && config.IgnoreSourceNull) return;

            if (sourceValue == null)
            {
                TargetProperty.SetValue(target, null);
                return;
            }
            object targetValue = TargetProperty.GetValue(target);
            if (targetValue == null && !TargetType.IsAbstract)
            {
                try
                {
                    targetValue = ObjectCreator.CreateInstance(TargetType);
                }
                catch
                {
                    return;
                }
            }
            TargetProperty.SetValue(target, targetValue);
            DataMapper.CopyNormal(sourceValue, SourceType, ref targetValue, TargetType, string.Format("{0}{1}.", higherName, PropertyName), config);
        }
    }
}
