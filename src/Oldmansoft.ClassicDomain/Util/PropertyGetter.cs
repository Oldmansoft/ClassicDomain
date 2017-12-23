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
        private Type PropertyType;

        private string PropertyName;

        /// <summary>
        /// 获值
        /// </summary>
        public Func<TCaller, TValue> Get { get; private set; }

        /// <summary>
        /// 创建属性获值器
        /// </summary>
        /// <param name="property"></param>
        public PropertyGetter(PropertyInfo property)
        {
            if (property == null) throw new ArgumentNullException();
            PropertyName = property.Name;
            PropertyType = property.PropertyType;
            Get = (Func<TCaller, TValue>)Delegate.CreateDelegate(typeof(Func<TCaller, TValue>), property.GetGetMethod(true));
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

        object IGetter.Get(object caller)
        {
            return Get((TCaller)caller);
        }
    }
}
