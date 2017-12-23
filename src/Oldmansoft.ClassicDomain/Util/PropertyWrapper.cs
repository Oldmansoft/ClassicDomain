using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    /// <summary>
    /// 属性包装器
    /// </summary>
    /// <typeparam name="TCaller"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class PropertyWrapper<TCaller, TValue> : IValue
    {
        private Type PropertyType;

        private string PropertyName;

        /// <summary>
        /// 获值
        /// </summary>
        public Func<TCaller, TValue> Get { get; private set; }

        /// <summary>
        /// 设值
        /// </summary>
        public Action<TCaller, TValue> Set { get; private set; }

        /// <summary>
        /// 创建属性包装器
        /// </summary>
        /// <param name="property"></param>
        public PropertyWrapper(PropertyInfo property)
        {
            if (property == null) throw new ArgumentNullException();
            PropertyName = property.Name;
            PropertyType = property.PropertyType;
            Get = (Func<TCaller, TValue>)Delegate.CreateDelegate(typeof(Func<TCaller, TValue>), property.GetGetMethod(true));
            Set = (Action<TCaller, TValue>)Delegate.CreateDelegate(typeof(Action<TCaller, TValue>), property.GetSetMethod(true));
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get
            {
                return PropertyName;
            }
        }

        /// <summary>
        /// 类型
        /// </summary>
        public Type Type
        {
            get
            {
                return PropertyType;
            }
        }

        object IGetter.Get(object caller)
        {
            return Get((TCaller)caller);
        }

        void ISetter.Set(object caller, object value)
        {
            Set((TCaller)caller, (TValue)value);
        }
    }
}
