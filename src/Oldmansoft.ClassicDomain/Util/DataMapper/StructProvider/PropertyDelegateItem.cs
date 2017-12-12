using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    /// <summary>
    /// 属性委托项
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class PropertyDelegateItem<TSource, TTarget, TValue>
    {
        /// <summary>
        /// 取值器
        /// </summary>
        public Func<TSource, TValue> Getter;

        /// <summary>
        /// 赋值器
        /// </summary>
        public Action<TTarget, TValue> Setter;

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="name"></param>
        public PropertyDelegateItem(string name)
        {
            Getter = Extends.CreateGetMethod<TSource, TValue>(name);
            Setter = Extends.CreateSetMethod<TTarget, TValue>(name);
        }
    }
}
