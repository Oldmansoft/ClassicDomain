using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class Extends
    {
        /// <summary>
        /// 分页
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IPagingCondition<TSource> Paging<TSource>(this IQueryable<TSource> source)
            where TSource : class
        {
            return new Util.Paging.PagingCondition<TSource>(source);
        }
        
        /// <summary>
        /// 复制到
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static TTarget MapTo<TSource, TTarget>(this TSource source, TTarget target)
        {
            if (source is Util.DataMapper) throw new ArgumentException("不能是 DataMapper 类型", "source");
            return Util.DataMapper.Map(source, target);
        }

        /// <summary>
        /// 复制
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TTarget MapTo<TTarget>(this object source)
        {
            if (source is Util.DataMapper) throw new ArgumentException("不能是 DataMapper 类型", "source");
            return Util.DataMapper.Map<TTarget>(source);
        }

        /// <summary>
        /// 创建属性的获值方法委托
        /// </summary>
        /// <typeparam name="TCaller"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Func<TCaller, TValue> CreateGetMethod<TCaller, TValue>(string name)
        {
            return (Func<TCaller, TValue>)Delegate.CreateDelegate(typeof(Func<TCaller, TValue>), typeof(TCaller).GetProperty(name).GetGetMethod(true));
        }

        /// <summary>
        /// 创建属性的赋值方法委托
        /// </summary>
        /// <typeparam name="TCaller"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Action<TCaller, TValue> CreateSetMethod<TCaller, TValue>(string name)
        {
            return (Action<TCaller, TValue>)Delegate.CreateDelegate(typeof(Action<TCaller, TValue>), typeof(TCaller).GetProperty(name).GetSetMethod(true));
        }
    }
}
