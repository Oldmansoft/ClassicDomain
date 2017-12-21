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
    public class PropertyWrapper<TCaller, TValue> : IPropertyValue
    {
        /// <summary>
        /// 获值
        /// </summary>
        public Func<TCaller, TValue> Get { get; private set; }

        /// <summary>
        /// 设置
        /// </summary>
        public Action<TCaller, TValue> Set { get; private set; }

        /// <summary>
        /// 创建属性包装器
        /// </summary>
        /// <param name="property"></param>
        public PropertyWrapper(PropertyInfo property)
        {
            Get = (Func<TCaller, TValue>)Delegate.CreateDelegate(typeof(Func<TCaller, TValue>), property.GetGetMethod(true));
            Set = (Action<TCaller, TValue>)Delegate.CreateDelegate(typeof(Action<TCaller, TValue>), property.GetSetMethod(true));
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
