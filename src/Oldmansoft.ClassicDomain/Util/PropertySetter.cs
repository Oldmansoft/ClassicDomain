using System;
using System.Reflection;

namespace Oldmansoft.ClassicDomain.Util
{
    /// <summary>
    /// 属性设值器
    /// </summary>
    /// <typeparam name="TCaller"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class PropertySetter<TCaller, TValue> : ISetter
    {
        private readonly Type PropertyType;

        private readonly string PropertyName;

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
