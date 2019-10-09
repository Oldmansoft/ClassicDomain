using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    /// <summary>
    /// 数据映射
    /// </summary>
    public class DataMapper
    {
        /// <summary>
        /// 复制到
        /// </summary>
        /// <param name="source">源对象</param>
        /// <param name="target">目标对象</param>
        /// <returns>返回目标对象</returns>
        public static void MapObject(object source, object target)
        {
            if (source == null || target == null) return;

            var sourceType = source.GetType();
            var targetType = target.GetType();
            var maps = Mapper.GetMapper(sourceType, targetType);
            for (var i = 0; i < maps.Length; i++)
            {
                maps[i].Map(source, target);
            }
        }

        /// <summary>
        /// 复制到
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="source">源对象</param>
        /// <param name="target">目标对象</param>
        /// <returns>返回目标对象</returns>
        public static TTarget Map<TSource, TTarget>(TSource source, TTarget target)
        {
            if (source == null || target == null) return default(TTarget);

            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);
            var maps = Mapper.GetMapper(sourceType, targetType);
            for (var i = 0; i < maps.Length; i++)
            {
                maps[i].Map(source, target);
            }
            return target;
        }

        /// <summary>
        /// 复制
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="source">源对象</param>
        /// <returns>返回目标对象</returns>
        public static TTarget Map<TTarget>(object source)
        {
            if (source == null) return default(TTarget);

            var sourceType = source.GetType();
            var targetType = typeof(TTarget);
            TTarget target;
            if (targetType.IsArray)
            {
                if (sourceType.IsArray)
                {
                    target = (TTarget)Activator.CreateInstance(targetType, (source as Array).Length);
                }
                else
                {
                    return default(TTarget);
                }
            }
            else
            {
                target = ObjectCreator.CreateInstance<TTarget>();
            }
            var maps = Mapper.GetMapper(sourceType, targetType);
            for (var i = 0; i < maps.Length; i++)
            {
                maps[i].Map(source, target);
            }
            return target;
        }

        /// <summary>
        /// 复制普通类
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sourceType"></param>
        /// <param name="target"></param>
        /// <param name="targetType"></param>
        internal static void NormalClassCopy(object source, Type sourceType, ref object target, Type targetType)
        {
            if (source == null)
            {
                target = null;
                return;
            }
            var maps = Mapper.GetMapper(sourceType, targetType);
            for (var i = 0; i < maps.Length; i++)
            {
                maps[i].Map(source, target);
            }
        }

        /// <summary>
        /// 复制项
        /// </summary>
        /// <param name="sourceItemType"></param>
        /// <param name="targetItemType"></param>
        /// <param name="isNormalClass"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        internal static object ItemValueCopy(Type sourceItemType, Type targetItemType, bool isNormalClass, object source)
        {
            if (isNormalClass)
            {
                if (targetItemType.IsAbstract) return null;
                object targetValue = null;
                try
                {
                    targetValue = ObjectCreator.CreateInstance(targetItemType);
                }
                catch
                {
                    return null;
                }
                NormalClassCopy(source, sourceItemType, ref targetValue, targetItemType);
                return targetValue;
            }

            if (sourceItemType.IsEnum && targetItemType.IsEnum)
            {
                return Enum.ToObject(targetItemType, (int)source);
            }

            if (sourceItemType.IsNullableEnum() && targetItemType.IsNullableEnum())
            {
                return Activator.CreateInstance(targetItemType, Enum.ToObject(targetItemType.GenericTypeArguments[0], (int)source));
            }

            if (sourceItemType == targetItemType)
            {
                return source;
            }
            return null;
        }
    }
}
