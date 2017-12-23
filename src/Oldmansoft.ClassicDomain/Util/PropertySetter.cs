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
        private Type PropertyType;

        private string PropertyName;

        /// <summary>
        /// 设值
        /// </summary>
        public Action<TCaller, TValue> Set { get; private set; }

        /// <summary>
        /// 创建属性设值器
        /// </summary>
        /// <param name="property"></param>
        public PropertySetter(PropertyInfo property)
        {
            if (property == null) throw new ArgumentNullException();
            PropertyName = property.Name;
            PropertyType = property.PropertyType;
            Set = (Action<TCaller, TValue>)Delegate.CreateDelegate(typeof(Action<TCaller, TValue>), property.GetSetMethod(true));
        }

        string IContent.Name
        {
            get
            {
                return PropertyName;
            }
        }

        Type IContent.Type
        {
            get
            {
                return PropertyType;
            }
        }

        void ISetter.Set(object caller, object value)
        {
            Set((TCaller)caller, (TValue)value);
        }
    }
}
