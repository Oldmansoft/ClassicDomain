using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    class SetGetValueProperty : IValueAction
    {
        private PropertyInfo SourceProperty;

        private PropertyInfo TargetProperty;

        public SetGetValueProperty(PropertyInfo sourceProperty, PropertyInfo targetProperty)
        {
            SourceProperty = sourceProperty;
            TargetProperty = targetProperty;
        }

        public object CreateTarget(object target, Type targetType)
        {
            return ObjectCreator.CreateInstance(targetType);
        }

        public object GetValue(object obj)
        {
            return SourceProperty.GetValue(obj);
        }

        public void SetValue<TTarget>(ref TTarget obj, object value)
        {
            TargetProperty.SetValue(obj, value);
        }
    }
}
