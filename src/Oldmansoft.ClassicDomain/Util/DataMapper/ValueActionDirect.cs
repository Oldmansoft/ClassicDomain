using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    class SetGetValueDirect : IValueAction
    {
        public object CreateTarget(object target, Type targetType)
        {
            return target;
        }

        public object GetValue(object obj)
        {
            return obj;
        }

        public void SetValue<TTarget>(ref TTarget obj, object value)
        {
            obj = (TTarget)value;
        }
    }
}
