using System;
using System.Collections.Concurrent;
using System.Globalization;

namespace Oldmansoft.ClassicDomain.Util
{
    /// <summary>
    /// 类型解析
    /// </summary>
    class TypeParse
    {
        private static readonly ConcurrentDictionary<Type, Func<string, object>> Store;

        static TypeParse()
        {
            Store = new ConcurrentDictionary<Type, Func<string, object>>();
        }

        /// <summary>
        /// 获取类型的字符串转换值方法
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Func<string, object> Get(Type type)
        {
            if (Store.TryGetValue(type, out Func<string, object> result))
            {
                return result;
            }
            result = GetFunc(type);
            Store.TryAdd(type, result);
            return result;
        }

        private static Func<string, object> GetFunc(Type type)
        {
            if (type == typeof(string))
            {
                return x => x;
            }
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return GetValue(type.GenericTypeArguments[0]);
            }
            return GetValue(type);
        }

        private static Func<string, object> GetValue(Type type)
        {
            if (type.IsEnum)
            {
                return x => Enum.Parse(type, x, true);
            }
            if (type == typeof(Guid))
            {
                return x => Guid.Parse(x);
            }
            if (type == typeof(DateTime))
            {
                return x => DateTime.Parse(x, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            }
            if (type == typeof(TimeSpan))
            {
                return x => TimeSpan.Parse(x, CultureInfo.InvariantCulture);
            }
            if (type == typeof(bool))
            {
                return x => bool.Parse(x);
            }
            if (type == typeof(byte))
            {
                return x => byte.Parse(x, CultureInfo.InvariantCulture);
            }
            if (type == typeof(char))
            {
                return x => char.Parse(x);
            }
            if (type == typeof(decimal))
            {
                return x => decimal.Parse(x, CultureInfo.InvariantCulture);
            }
            if (type == typeof(double))
            {
                return x => double.Parse(x, CultureInfo.InvariantCulture);
            }
            if (type == typeof(float))
            {
                return x => float.Parse(x, CultureInfo.InvariantCulture);
            }
            if (type == typeof(int))
            {
                return x => int.Parse(x, CultureInfo.InvariantCulture);
            }
            if (type == typeof(long))
            {
                return x => long.Parse(x, CultureInfo.InvariantCulture);
            }
            if (type == typeof(sbyte))
            {
                return x => sbyte.Parse(x, CultureInfo.InvariantCulture);
            }
            if (type == typeof(short))
            {
                return x => short.Parse(x, CultureInfo.InvariantCulture);
            }
            if (type == typeof(uint))
            {
                return x => uint.Parse(x, CultureInfo.InvariantCulture);
            }
            if (type == typeof(ulong))
            {
                return x => ulong.Parse(x, CultureInfo.InvariantCulture);
            }
            if (type == typeof(ushort))
            {
                return x => ushort.Parse(x, CultureInfo.InvariantCulture);
            }
            throw new NotSupportedException(string.Format("不支持此类型 {0} 转换数据", type.FullName));
        }
    }
}
