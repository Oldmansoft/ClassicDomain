using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    /// <summary>
    /// 数据映射器
    /// </summary>
    public class DataMapper
    {
        /// <summary>
        /// 忽略属性
        /// </summary>
        private HashSet<string> IgnoreProperty { get; set; }

        /// <summary>
        /// 是否深拷贝
        /// </summary>
        public bool IsDeepCopy { get; set; }

        /// <summary>
        /// 创建数据映射
        /// 默认不进入深拷贝
        /// </summary>
        public DataMapper(bool isDeepCopy = false)
        {
            IgnoreProperty = new HashSet<string>();
            IsDeepCopy = isDeepCopy;
        }

        /// <summary>
        /// 添加忽略属性
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="keyExpression"></param>
        /// <returns></returns>
        public DataMapper AddIgnore<TEntity>(Expression<Func<TEntity, object>> keyExpression)
        {
            IgnoreProperty.Add(keyExpression.GetProperty().Name);
            return this;
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
            return Copy(source, target, string.Empty);
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
            return Copy(source, target, string.Empty);
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
            if (source == null || target == null) return default(TTarget);

            var sourceType = source.GetType();
            var targetType = target.GetType();

            foreach (var sourcePropertyInfo in TypePublicInstanceStore.GetPropertys(sourceType))
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
                    if (sourceValue == null) continue;
                    if (ArrayClassCopy(sourcePropertyInfo, targetPropertyInfo, sourceValue, target)) continue;
                }

                if (new Type[] { sourcePropertyInfo.PropertyType, targetPropertyInfo.PropertyType }.IsGenericEnumerable())
                {
                    if (!IsDeepCopy) continue;
                    object sourceValue = sourcePropertyInfo.GetValue(source);
                    if (sourceValue == null) continue;
                    if (ListClassCopy(sourcePropertyInfo, targetPropertyInfo, sourceValue, target)) continue;
                    if (DictionaryClassCopy(sourcePropertyInfo, targetPropertyInfo, sourceValue, target)) continue;
                }

                if (new Type[] { sourcePropertyInfo.PropertyType, targetPropertyInfo.PropertyType }.IsNormalClass())
                {
                    if (!IsDeepCopy) continue;
                    object sourceValue = sourcePropertyInfo.GetValue(source);
                    if (sourceValue == null) continue;
                    NormalClassCopy(target, refPropertyName, sourcePropertyInfo, targetPropertyInfo, sourceValue);
                    continue;
                }

                if (sourcePropertyInfo.PropertyType.IsEnum && targetPropertyInfo.PropertyType.IsEnum)
                {
                    targetPropertyInfo.SetValue(target, (int)sourcePropertyInfo.GetValue(source));
                    continue;
                }

                if (sourcePropertyInfo.PropertyType.IsNullableEnum() && targetPropertyInfo.PropertyType.IsNullableEnum())
                {
                    var sourceValue = sourcePropertyInfo.GetValue(source);
                    if (sourceValue == null) continue;
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
                    targetValue = Activator.CreateInstance(targetItemType);
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
            if (source == null) return true;

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
            if (!sourcePropertyInfo.PropertyType.GetInterfaces().Contains(typeof(System.Collections.IEnumerable))) return false;
            if (!sourcePropertyInfo.PropertyType.IsGenericType) return false;
            if (!targetPropertyInfo.PropertyType.GetInterfaces().Contains(typeof(System.Collections.IEnumerable))) return false;
            if (!targetPropertyInfo.PropertyType.IsGenericType) return false;

            var sourceItemType = sourcePropertyInfo.PropertyType.GetGenericArguments()[0];
            var targetItemType = targetPropertyInfo.PropertyType.GetGenericArguments()[0];
            var targetType = typeof(List<>).MakeGenericType(targetItemType);

            if (targetType != targetPropertyInfo.PropertyType && !targetType.GetInterfaces().Contains(targetPropertyInfo.PropertyType)) return false;
            var source = (sourceValue as System.Collections.IEnumerable);
            if (source == null) return true;

            var isNormalClass = sourceItemType.IsNormalClass() && targetItemType.IsNormalClass();
            var targetValue = Activator.CreateInstance(targetType) as System.Collections.IList;
            foreach (var item in source)
            {
                targetValue.Add(ValueCopy(sourceItemType, targetItemType, isNormalClass, item));
            }
            targetPropertyInfo.SetValue(target, targetValue);
            return true;
        }

        private bool DictionaryClassCopy(PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo, object sourceValue, object target)
        {
            if (!sourcePropertyInfo.PropertyType.GetInterfaces().Contains(typeof(System.Collections.IDictionary))) return false;
            if (!sourcePropertyInfo.PropertyType.IsGenericType) return false;
            if (!targetPropertyInfo.PropertyType.GetInterfaces().Contains(typeof(System.Collections.IDictionary))) return false;
            if (!targetPropertyInfo.PropertyType.IsGenericType) return false;

            var sourceKeyType = sourcePropertyInfo.PropertyType.GetGenericArguments()[0];
            var sourceValueType = sourcePropertyInfo.PropertyType.GetGenericArguments()[1];
            var targetKeyType = targetPropertyInfo.PropertyType.GetGenericArguments()[0];
            var targetValueType = targetPropertyInfo.PropertyType.GetGenericArguments()[1];

            if (sourceKeyType != targetKeyType) return false;

            var targetType = typeof(Dictionary<,>).MakeGenericType(targetKeyType, targetValueType);

            if (!targetType.GetInterfaces().Contains(targetPropertyInfo.PropertyType)) return false;

            var isNormalClass = sourceValueType.IsNormalClass() && targetValueType.IsNormalClass();
            var targetValue = Activator.CreateInstance(targetType) as System.Collections.IDictionary;
            var source = sourceValue as System.Collections.IDictionary;
            if (source == null) return true;
            foreach (var key in source.Keys)
            {
                targetValue.Add(key, ValueCopy(sourceValueType, targetValueType, isNormalClass, source[key]));
            }
            targetPropertyInfo.SetValue(target, targetValue);
            return true;
        }

        private void NormalClassCopy(object target, string refPropertyName, PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo, object sourceValue)
        {
            if (sourceValue == null) return;
            object targetValue = targetPropertyInfo.GetValue(target);
            if (targetValue == null && !targetPropertyInfo.PropertyType.IsAbstract)
            {
                try
                {
                    targetValue = Activator.CreateInstance(targetPropertyInfo.PropertyType);
                    targetPropertyInfo.SetValue(target, targetValue);
                }
                catch { }
            }
            Copy(sourceValue, targetValue, string.Format("{0}{1}.", refPropertyName, sourcePropertyInfo.Name));
        }
    }
}
