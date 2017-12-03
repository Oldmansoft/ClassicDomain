﻿using System;
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
        public override void Map<TTarget>(string higherName, object source, ref TTarget target, MapConfig config)
        {
            var sourceValue = ValueAction.GetValue(source);
            if (sourceValue == null && config.IgnoreSourceNull) return;

            var currentSource = sourceValue as IDictionary;
            if (currentSource == null)
            {
                ValueAction.SetValue(ref target, null);
                return;
            }

            var sourceKeyType = SourceType.GetGenericArguments()[0];
            var sourceValueType = SourceType.GetGenericArguments()[1];
            var targetKeyType = TargetType.GetGenericArguments()[0];
            var targetValueType = TargetType.GetGenericArguments()[1];

            var targetType = typeof(Dictionary<,>).MakeGenericType(targetKeyType, targetValueType);
            var isNormalClass = sourceValueType.IsNormalClass() && targetValueType.IsNormalClass();
            var targetValue = ValueAction.CreateTarget(target, targetType) as IDictionary;
            foreach (var key in currentSource.Keys)
            {
                targetValue.Add(key, DataMapper.ItemValueCopy(sourceValueType, targetValueType, isNormalClass, currentSource[key], config));
            }
            ValueAction.SetValue(ref target, targetValue);
            return;
        }
    }
}
