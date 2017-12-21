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
    public class PropertyWrapper<TCaller, TValue> : IGetter, ISetter
    {
        private Func<TCaller, TValue> Getter;

        private Action<TCaller, TValue> Setter;

        /// <summary>
        /// 创建属性包装器
        /// </summary>
        /// <param name="property"></param>
        public PropertyWrapper(PropertyInfo property)
        {
            Getter = (Func<TCaller, TValue>)Delegate.CreateDelegate(typeof(Func<TCaller, TValue>), property.GetGetMethod(true));
            Setter = (Action<TCaller, TValue>)Delegate.CreateDelegate(typeof(Action<TCaller, TValue>), property.GetSetMethod(true));
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="caller"></param>
        /// <returns></returns>
        public object Get(object caller)
        {
            return Getter((TCaller)caller);
        }

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="value"></param>
        public void Set(object caller, object value)
        {
            Setter((TCaller)caller, (TValue)value);
        }
    }
}
