using System;
using System.Collections;
using System.Reflection;

namespace Oldmansoft.ClassicDomain.Util
{
    class MapListProperty : MapProperty
    {
        private Type SourceItemType;

        private Type TargetItemType;

        private bool IsNormalClass;

        public override IMap Init(Type sourceType, Type targetType, PropertyInfo sourceProperty, PropertyInfo targetProperty)
        {
            base.Init(sourceType, targetType, sourceProperty, targetProperty);
            SourceItemType = SourcePropertyType.GetGenericArguments()[0];
            TargetItemType = TargetPropertyType.GetGenericArguments()[0];
            IsNormalClass = SourceItemType.IsNormalClass() && TargetItemType.IsNormalClass();
            return this;
        }

        public override void Map(object source, object target)
        {
            var sourceValue = (Getter.Get(source) as IEnumerable);
            if (sourceValue == null)
            {
                Setter.Set(target, null);
                return;
            }

            IList targetValue;
            try
            {
                targetValue = ObjectCreator.CreateInstance(TargetPropertyType) as IList;
            }
            catch (ClassicDomainException)
            {
                return;
            }

            foreach (var item in sourceValue)
            {
                targetValue.Add(DataMapper.ItemValueCopy(SourceItemType, TargetItemType, IsNormalClass, item));
            }
            Setter.Set(target, targetValue);
        }
    }
}
