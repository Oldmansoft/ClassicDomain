using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    /// <summary>
    /// 属性设值器
    /// </summary>
    /// <typeparam name="TCaller"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class PropertySetter<TCaller, TValue> : ISetter
    {
        private Action<TCaller, TValue> Setter;

        /// <summary>
        /// 创建属性设值器
        /// </summary>
        /// <param name="property"></param>
        public PropertySetter(PropertyInfo property)
        {
            if (property == null) throw new ArgumentNullException();
            Setter = (Action<TCaller, TValue>)Delegate.CreateDelegate(typeof(Action<TCaller, TValue>), property.GetSetMethod(true));
        }

        void ISetter.Set(object caller, object value)
        {
            Setter((TCaller)caller, (TValue)value);
        }
    }
}
