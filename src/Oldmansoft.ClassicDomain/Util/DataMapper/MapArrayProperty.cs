using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    class MapArrayProperty : MapProperty
    {
        private Type SourceItemType;

        private Type TargetItemType;

        private bool IsNormalClass;

        public override IMap Init(Type sourceType, Type targetType, PropertyInfo sourceProperty, PropertyInfo targetProperty)
        {
            base.Init(sourceType, targetType, sourceProperty, targetProperty);
            SourceItemType = SourcePropertyType.GetMethod("Set").GetParameters()[1].ParameterType;
            TargetItemType = TargetPropertyType.GetMethod("Set").GetParameters()[1].ParameterType;
            IsNormalClass = SourceItemType.IsNormalClass() && TargetItemType.IsNormalClass();
            return this;
        }

        public override void Map(object source, ref object target)
        {
            var sourceValue = Getter.Get(source);
            var currentSource = sourceValue as Array;
            if (currentSource == null)
            {
                Setter.Set(target, null);
                return;
            }

            var targetValue = Array.CreateInstance(TargetItemType, currentSource.Length);
            int index = 0;
            foreach (var item in currentSource)
            {
                targetValue.SetValue(DataMapper.ItemValueCopy(SourceItemType, TargetItemType, IsNormalClass, item), index);
                index++;
            }
            Setter.Set(target, targetValue);
        }
    }
}
