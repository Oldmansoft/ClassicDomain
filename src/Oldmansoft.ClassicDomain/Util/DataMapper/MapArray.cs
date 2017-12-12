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
        private Type SourceItemType;

        private Type TargetItemType;

        private bool IsNormalClass;

        public override IMap Init(Type sourceType, Type targetType)
        {
            SourceItemType = sourceType.GetMethod("Set").GetParameters()[1].ParameterType;
            TargetItemType = targetType.GetMethod("Set").GetParameters()[1].ParameterType;
            IsNormalClass = SourceItemType.IsNormalClass() && TargetItemType.IsNormalClass();
            return base.Init(sourceType, targetType);
        }

        public override void Map(object source, object target)
        {
            var currentSource = source as Array;
            var targetValue = target as Array;
            int index = 0;
            foreach (var item in currentSource)
            {
                targetValue.SetValue(DataMapper.ItemValueCopy(SourceItemType, TargetItemType, IsNormalClass, item), index);
                index++;
            }
        }
    }
}
