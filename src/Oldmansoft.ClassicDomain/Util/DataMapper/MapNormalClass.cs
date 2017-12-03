using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    class MapNormalClass : MapContent
    {
        public override void Map<TTarget>(string higherName, object source, ref TTarget target, MapConfig config)
        {
            object sourceValue = ValueAction.GetValue(source);
            if (sourceValue == null && config.IgnoreSourceNull) return;

            if (sourceValue == null)
            {
                ValueAction.SetValue(ref target, null);
                return;
            }
            object targetValue = ValueAction.GetValue(target);
            if (targetValue == null && !TargetType.IsAbstract)
            {
                try
                {
                    targetValue = ObjectCreator.CreateInstance(TargetType);
                    ValueAction.SetValue(ref target, targetValue);
                }
                catch { }
            }
            DataMapper.CopyNormal(sourceValue, SourceType, ref targetValue, TargetType, string.Format("{0}{1}.", higherName, PropertyName), config);
        }
    }
}
