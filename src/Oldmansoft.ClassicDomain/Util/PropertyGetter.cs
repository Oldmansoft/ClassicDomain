using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    /// <summary>
    /// 属性获值器
    /// </summary>
    /// <typeparam name="TCaller"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class PropertyGetter<TCaller, TValue> : IGetter
    {
        private Func<TCaller, TValue> Getter;

        /// <summary>
        /// 创建属性获值器
        /// </summary>
        /// <param name="property"></param>
        public PropertyGetter(PropertyInfo property)
        {
            Getter = (Func<TCaller, TValue>)Delegate.CreateDelegate(typeof(Func<TCaller, TValue>), property.GetGetMethod(true));
        }

        object IGetter.Get(object caller)
        {
            return Getter((TCaller)caller);
        }
    }
}
