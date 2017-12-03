using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    interface IValueAction
    {
        void SetValue<TTarget>(ref TTarget obj, object value);

        object GetValue(object obj);

        object CreateTarget(object target, Type targetType);
    }
}
