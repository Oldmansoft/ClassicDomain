using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Library
{
    class Serializer
    {
        public static string Serialize<T>(T value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            return new ServiceStack.Text.JsonSerializer<T>().SerializeToString(value);
        }

        public static T Deserialize<T>(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return default(T);
            }
            return ServiceStack.Text.JsonSerializer.DeserializeFromString<T>(value);
        }

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
