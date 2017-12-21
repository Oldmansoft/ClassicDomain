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

        /// <summary>
        /// 忽略属性
        /// </summary>
        private HashSet<string> IgnoreProperty { get; set; }

        /// <summary>
        /// 是否深拷贝
        /// </summary>
        public bool IsDeepCopy { get; set; }

        /// <summary>
        /// 是否忽略空值
        /// </summary>
        public bool IsIgnoreNull { get; set; }

        /// <summary>
        /// 创建数据映射
        /// </summary>
        [Obsolete("建议使用静态方法 Map 来实现")]
        public DataMapper(bool isDeepCopy = true)
        {
            IgnoreProperty = new HashSet<string>();
            IsDeepCopy = isDeepCopy;
            IsIgnoreNull = false;
        }

        /// <summary>
        /// 添加忽略属性
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="keyExpression"></param>
        /// <returns></returns>
        [Obsolete("请使用 SetIgnore 方法代替")]
        public DataMapper AddIgnore<TEntity>(Expression<Func<TEntity, object>> keyExpression)
        {
            IgnoreProperty.Add(keyExpression.GetProperty().Name);
            return this;
        }

        /// <summary>
        /// 设置忽略属性的实体
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public IPropertyIgnore<TEntity> SetIgnore<TEntity>()
        {
            return new PropertyIgnore<TEntity>(IgnoreProperty);
        }

        /// <summary>
        /// 复制
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="source">源对象</param>
        /// <param name="target">目标对象</param>
        /// <returns>返回目标对象</returns>
        [Obsolete("请使用 CopyTo 方法代替")]
        public TTarget Copy<TTarget>(object source, TTarget target)
        {
            return CopyTo(source, target);
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
            if (source == null || target == null) return default(TTarget);
            if (source is Array)
            {
                if (target is Array)
                {
                    CopyArray(source as Array, target as Array);
                    return target;
                }
                throw new ArgumentException("必须是数组", "target");
            }
            if (source is IEnumerable && target is IList)
            {
                CopyList(source as IEnumerable, target as IList);
                return target;
            }
            if (source is IDictionary && target is IDictionary)
            {
                CopyDictionary(source as IDictionary, target as IDictionary);
                return target;
            }
            return Copy(source, target, string.Empty);
        }

        /// <summary>
        /// 复制数组
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private void CopyArray(Array source, Array target)
        {
            var sourceType = source.GetType();
            var targetType = target.GetType();

            var sourceItemType = sourceType.GetMethod("Set").GetParameters()[1].ParameterType;
            var targetItemType = targetType.GetMethod("Set").GetParameters()[1].ParameterType;

            if (sourceItemType.IsNormalClass() != targetItemType.IsNormalClass()) return;
            var isNormalClass = sourceItemType.IsNormalClass();
            int index = 0;
            foreach (var item in source)
            {
                if (target.Length == index) break;
                target.SetValue(ValueCopy(sourceItemType, targetItemType, isNormalClass, item), index);
                index++;
            }
        }

        /// <summary>
        /// 复制列表
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private void CopyList(IEnumerable source, IList target)
        {
            var sourceType = source.GetType();
            var targetType = target.GetType();
            if (!sourceType.IsGenericType || !targetType.IsGenericType) return;

            var sourceItemType = sourceType.GetGenericArguments()[0];
            var targetItemType = targetType.GetGenericArguments()[0];

            if (sourceItemType.IsNormalClass() != targetItemType.IsNormalClass()) return;
            var isNormalClass = sourceItemType.IsNormalClass();
            foreach (var item in source)
            {
                target.Add(ValueCopy(sourceItemType, targetItemType, isNormalClass, item));
            }
        }

        /// <summary>
        /// 复制键值集
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private void CopyDictionary(IDictionary source, IDictionary target)
        {
            var sourceType = source.GetType();
            var targetType = target.GetType();
            if (!sourceType.IsGenericType || !targetType.IsGenericType) return;

            var sourceKeyType = sourceType.GetGenericArguments()[0];
            var sourceValueType = sourceType.GetGenericArguments()[1];
            var targetKeyType = targetType.GetGenericArguments()[0];
            var targetValueType = targetType.GetGenericArguments()[1];

            if (sourceKeyType != targetKeyType) return;

            if (sourceValueType.IsNormalClass() != targetValueType.IsNormalClass()) return;
            var isNormalClass = sourceValueType.IsNormalClass();
            foreach (var key in source.Keys)
            {
                target.Add(key, ValueCopy(sourceValueType, targetValueType, isNormalClass, source[key]));
            }
        }

        /// <summary>
        /// 复制
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="refPropertyName"></param>
        /// <returns></returns>
        private TTarget Copy<TTarget>(object source, TTarget target, string refPropertyName)
        {
            if (source == null || target == null) return target;
            var sourceType = source.GetType();
            var targetType = target.GetType();

            foreach (var sourcePropertyInfo in TypePublicInstancePropertyInfoStore.GetPropertys(sourceType))
            {
                if (!sourcePropertyInfo.CanRead) continue;
                if (IgnoreProperty.Contains(string.Format("{0}{1}", refPropertyName, sourcePropertyInfo.Name))) continue;

                var targetPropertyInfo = targetType.GetProperty(sourcePropertyInfo.Name, BindingFlags.Public | BindingFlags.Instance);
                if (targetPropertyInfo == null) continue;
                if (!targetPropertyInfo.CanWrite) continue;

                if (sourcePropertyInfo.PropertyType.IsArray && targetPropertyInfo.PropertyType.IsArray)
                {
                    if (!IsDeepCopy) continue;
                    object sourceValue = sourcePropertyInfo.GetValue(source);
                    if (sourceValue == null && IsIgnoreNull) continue;
                    if (ArrayClassCopy(sourcePropertyInfo, targetPropertyInfo, sourceValue, target)) continue;
                }

                if (new Type[] { sourcePropertyInfo.PropertyType, targetPropertyInfo.PropertyType }.IsGenericEnumerable())
                {
                    if (!IsDeepCopy) continue;
                    object sourceValue = sourcePropertyInfo.GetValue(source);
                    if (sourceValue == null && IsIgnoreNull) continue;
                    if (ListClassCopy(sourcePropertyInfo, targetPropertyInfo, sourceValue, target)) continue;
                    if (DictionaryClassCopy(sourcePropertyInfo, targetPropertyInfo, sourceValue, target)) continue;
                }

                if (new Type[] { sourcePropertyInfo.PropertyType, targetPropertyInfo.PropertyType }.IsNormalClass())
                {
                    if (!IsDeepCopy) continue;
                    object sourceValue = sourcePropertyInfo.GetValue(source);
                    if (sourceValue == null && IsIgnoreNull) continue;
                    NormalClassCopy(target, refPropertyName, sourcePropertyInfo, targetPropertyInfo, sourceValue);
                    continue;
                }

                if (sourcePropertyInfo.PropertyType.IsEnum && targetPropertyInfo.PropertyType.IsEnum)
                {
                    try
                    {
                        targetPropertyInfo.SetValue(target, Enum.Parse(targetPropertyInfo.PropertyType, sourcePropertyInfo.GetValue(source).ToString()));
                    }
                    catch (ArgumentException) { }
                    continue;
                }

                if (sourcePropertyInfo.PropertyType.IsNullableEnum() && targetPropertyInfo.PropertyType.IsNullableEnum())
                {
                    var sourceValue = sourcePropertyInfo.GetValue(source);
                    if (sourceValue == null && IsIgnoreNull) continue;
                    if (sourceValue == null)
                    {
                        targetPropertyInfo.SetValue(target, null);
                        continue;
                    }
                    var targetValue = Enum.ToObject(targetPropertyInfo.PropertyType.GenericTypeArguments[0], (int)sourceValue);
                    targetPropertyInfo.SetValue(target, Activator.CreateInstance(targetPropertyInfo.PropertyType, targetValue));
                    continue;
                }

                if (sourcePropertyInfo.PropertyType != targetPropertyInfo.PropertyType) continue;
                targetPropertyInfo.SetValue(target, sourcePropertyInfo.GetValue(source));
            }

            return target;
        }

        private object ValueCopy(Type sourceItemType, Type targetItemType, bool isNormalClass, object source)
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
                return CopyTo(source, targetValue);
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

        private bool ArrayClassCopy(PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo, object sourceValue, object target)
        {
            if (!sourcePropertyInfo.PropertyType.IsArray) return false;
            if (!targetPropertyInfo.PropertyType.IsArray) return false;

            var sourceItemType = sourcePropertyInfo.PropertyType.GetMethod("Set").GetParameters()[1].ParameterType;
            var targetItemType = targetPropertyInfo.PropertyType.GetMethod("Set").GetParameters()[1].ParameterType;

            var isNormalClass = sourceItemType.IsNormalClass() && targetItemType.IsNormalClass();
            var source = sourceValue as Array;
            if (source == null)
            {
                targetPropertyInfo.SetValue(target, null);
                return true;
            }

            var targetValue = targetPropertyInfo.PropertyType.InvokeMember("Set", BindingFlags.CreateInstance, null, null, new object[] { source.Length }) as Array;
            var method = targetPropertyInfo.PropertyType.GetMethod("SetValue", new Type[] { typeof(object), typeof(int) });
            int index = 0;
            foreach (var item in source)
            {
                method.Invoke(targetValue, new object[] { ValueCopy(sourceItemType, targetItemType, isNormalClass, item), index });
                index++;
            }
            targetPropertyInfo.SetValue(target, targetValue);
            return true;
        }

        private bool ListClassCopy(PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo, object sourceValue, object target)
        {
            if (!sourcePropertyInfo.PropertyType.GetInterfaces().Contains(typeof(IEnumerable))) return false;
            if (!sourcePropertyInfo.PropertyType.IsGenericType) return false;
            if (!targetPropertyInfo.PropertyType.GetInterfaces().Contains(typeof(IEnumerable))) return false;
            if (!targetPropertyInfo.PropertyType.IsGenericType) return false;

            var sourceItemType = sourcePropertyInfo.PropertyType.GetGenericArguments()[0];
            var targetItemType = targetPropertyInfo.PropertyType.GetGenericArguments()[0];
            var targetType = typeof(List<>).MakeGenericType(targetItemType);

            if (targetType != targetPropertyInfo.PropertyType && !targetType.GetInterfaces().Contains(targetPropertyInfo.PropertyType)) return false;
            var source = (sourceValue as IEnumerable);
            if (source == null)
            {
                targetPropertyInfo.SetValue(target, null);
                return true;
            }

            var isNormalClass = sourceItemType.IsNormalClass() && targetItemType.IsNormalClass();
            var targetValue = ObjectCreator.CreateInstance(targetType) as IList;
            foreach (var item in source)
            {
                targetValue.Add(ValueCopy(sourceItemType, targetItemType, isNormalClass, item));
            }
            targetPropertyInfo.SetValue(target, targetValue);
            return true;
        }

        private bool DictionaryClassCopy(PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo, object sourceValue, object target)
        {
            if (!sourcePropertyInfo.PropertyType.GetInterfaces().Contains(typeof(IDictionary))) return false;
            if (!sourcePropertyInfo.PropertyType.IsGenericType) return false;
            if (!targetPropertyInfo.PropertyType.GetInterfaces().Contains(typeof(IDictionary))) return false;
            if (!targetPropertyInfo.PropertyType.IsGenericType) return false;

            var sourceKeyType = sourcePropertyInfo.PropertyType.GetGenericArguments()[0];
            var sourceValueType = sourcePropertyInfo.PropertyType.GetGenericArguments()[1];
            var targetKeyType = targetPropertyInfo.PropertyType.GetGenericArguments()[0];
            var targetValueType = targetPropertyInfo.PropertyType.GetGenericArguments()[1];

            if (sourceKeyType != targetKeyType) return false;

            var targetType = typeof(Dictionary<,>).MakeGenericType(targetKeyType, targetValueType);

            if (targetType != targetPropertyInfo.PropertyType && !targetType.GetInterfaces().Contains(targetPropertyInfo.PropertyType)) return false;

            var isNormalClass = sourceValueType.IsNormalClass() && targetValueType.IsNormalClass();
            var targetValue = ObjectCreator.CreateInstance(targetType) as IDictionary;
            var source = sourceValue as IDictionary;
            if (source == null)
            {
                targetPropertyInfo.SetValue(target, null);
                return true;
            }
            foreach (var key in source.Keys)
            {
                targetValue.Add(key, ValueCopy(sourceValueType, targetValueType, isNormalClass, source[key]));
            }
            targetPropertyInfo.SetValue(target, targetValue);
            return true;
        }

        private void NormalClassCopy(object target, string refPropertyName, PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo, object sourceValue)
        {
            if (sourceValue == null)
            {
                targetPropertyInfo.SetValue(target, null);
                return;
            }
            object targetValue = targetPropertyInfo.GetValue(target);
            if (targetValue == null && !targetPropertyInfo.PropertyType.IsAbstract)
            {
                try
                {
                    targetValue = ObjectCreator.CreateInstance(targetPropertyInfo.PropertyType);
                    targetPropertyInfo.SetValue(target, targetValue);
                }
                catch { }
            }
            Copy(sourceValue, targetValue, string.Format("{0}{1}.", refPropertyName, sourcePropertyInfo.Name));
        }
    }
}
