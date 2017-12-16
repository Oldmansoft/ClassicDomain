using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    class MapList : MapContent
    {
        private Type SourceItemType;

        private Type TargetItemType;

        private bool IsNormalClass;

        public override IMap Init(Type sourceType, Type targetType)
        {
            SourceItemType = sourceType.GetGenericArguments()[0];
            TargetItemType = targetType.GetGenericArguments()[0];
            IsNormalClass = SourceItemType.IsNormalClass() && TargetItemType.IsNormalClass();
            return base.Init(sourceType, targetType);
        }

        public override void Map(object source, object target)
        {
            var sourceValue = (source as IEnumerable);
            var targetValue = target as IList;
            foreach (var item in sourceValue)
            {
                targetValue.Add(DataMapper.ItemValueCopy(SourceItemType, TargetItemType, IsNormalClass, item));
            }
        }
    }
}
