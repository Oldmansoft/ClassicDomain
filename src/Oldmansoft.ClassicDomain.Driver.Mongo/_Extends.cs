using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.Mongo
{
    static class Extends
    {
        public static string GetDatabase(this Uri source)
        {
            return source.AbsolutePath.Substring(1);
        }

        public static string GetHost(this MongoDB.Driver.MongoServerSettings source)
        {
            var builder = new StringBuilder();
            foreach (var item in source.Servers)
            {
                if (builder.Length > 0) builder.Append(",");
                builder.Append(item.Host);
                builder.Append(":");
                builder.Append(item.Port);
            }
            return builder.ToString();
        }
    }
}
