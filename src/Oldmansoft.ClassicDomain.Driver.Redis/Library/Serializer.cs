using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Library
{
    /// <summary>
    /// 序列化器
    /// </summary>
    public class Serializer
    {
        /// <summary>
        /// 序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Serialize<T>(T value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            return new ServiceStack.Text.JsonSerializer<T>().SerializeToString(value);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return default(T);
            }
            return ServiceStack.Text.JsonSerializer.DeserializeFromString<T>(value);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object Deserialize(string value, Type type)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            return ServiceStack.Text.JsonSerializer.DeserializeFromString(value, type);
        }
    }
}
