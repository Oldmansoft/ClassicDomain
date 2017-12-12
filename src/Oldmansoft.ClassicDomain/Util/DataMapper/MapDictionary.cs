using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    class MapDictionary : MapContent
    {
        private Type SourceValueType;

        private Type TargetValueType;

        private bool IsNormalClass;

        public override IMap Init(Type sourceType, Type targetType)
        {
            SourceValueType = sourceType.GetGenericArguments()[1];
            TargetValueType = targetType.GetGenericArguments()[1];
            IsNormalClass = SourceValueType.IsNormalClass() && TargetValueType.IsNormalClass();
            return base.Init(sourceType, targetType);
        }

        public override void Map(object source, object target)
        {
            var currentSource = source as IDictionary;
            var targetValue = target as IDictionary;
            foreach (var key in currentSource.Keys)
            {
                targetValue.Add(key, DataMapper.ItemValueCopy(SourceValueType, TargetValueType, IsNormalClass, currentSource[key]));
            }
        }
    }
}
