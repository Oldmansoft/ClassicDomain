using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        /// 复制
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="source">源对象</param>
        /// <param name="target">目标对象</param>
        /// <param name="config">配置</param>
        /// <returns>返回目标对象</returns>
        public static TTarget Map<TSource, TTarget>(TSource source, TTarget target, MapConfig config = null)
        {
            if (source == null || target == null) return default(TTarget);
            if (config == null) config = MapConfig.Empty;

            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);

            var targetObject = (object)target;
            CopyNormal(source, sourceType, ref targetObject, targetType, string.Empty, config);
            return target;
        }
        
        internal static void CopyNormal(object source, Type sourceType, ref object target, Type targetType, string higherName, MapConfig config)
        {
            if (source == null)
            {
                target = null;
                return;
            }
            var maps = Mapper.GetMapper(sourceType, targetType, higherName, config);
            for (var i = 0; i < maps.Count; i++)
            {
                maps[i].Map(higherName, source, ref target, config);
            }
        }

        /// <summary>
        /// 复制项
        /// </summary>
        /// <param name="sourceItemType"></param>
        /// <param name="targetItemType"></param>
        /// <param name="isNormalClass"></param>
        /// <param name="source"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        internal static object ItemValueCopy(Type sourceItemType, Type targetItemType, bool isNormalClass, object source, MapConfig config)
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
                CopyNormal(source, sourceItemType, ref targetValue, targetItemType, string.Empty, config);
                return targetValue;
            }

            if (sourceItemType.IsEnum && targetItemType.IsEnum)
            {
                return (int)source;
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

        private MapConfig Config { get; set; }

        /// <summary>
        /// 是否深拷贝
        /// </summary>
        public bool IsDeepCopy { get { return Config.DeepCopy; } set { Config.DeepCopy = value; } }

        /// <summary>
        /// 是否忽略空值
        /// </summary>
        public bool IsIgnoreNull { get { return Config.IgnoreSourceNull; } set { Config.IgnoreSourceNull = value; } }

        /// <summary>
        /// 创建数据映射
        /// </summary>
        public DataMapper(bool isDeepCopy = true)
        {
            Config = new MapConfig();
            Config.DeepCopy = IsDeepCopy;
        }

        /// <summary>
        /// 设置忽略属性的实体
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public IPropertyIgnore<TEntity> SetIgnore<TEntity>()
        {
            return Config.SetIgnore<TEntity>();
        }

        /// <summary>
        /// 分页列表复制到
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="source">源对象</param>
        /// <param name="target">目标对象</param>
        /// <returns>返回目标对象</returns>
        public TTarget CopyTo<TSource, TTarget>(TSource source, TTarget target)
        {
            return Map(source, target, Config);
        }
    }
}
