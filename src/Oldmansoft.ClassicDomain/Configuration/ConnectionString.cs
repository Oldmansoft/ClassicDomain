using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Configuration
{
    /// <summary>
    /// 连接串
    /// </summary>
    public class ConnectionString
    {
        private static Dictionary<string, PropertyInfo> Propertys { get; set; }

        static ConnectionString()
        {
            Propertys = new Dictionary<string, PropertyInfo>(StringComparer.CurrentCultureIgnoreCase);
            Type type = typeof(ConnectionString);
            foreach (var item in type.GetProperties())
            {
                if (item.Name == "Name") continue;
                if (item.Name == "ProviderName") continue;
                if (item.Name == "DataSource") continue;
                Propertys.Add(item.Name, item);
            }
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 提供者名称
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// 数据源
        /// </summary>
        public DataSourceSet DataSource { get; set; }

        /// <summary>
        /// 数据库名称
        /// </summary>
        public string InitialCatalog { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 创建连接串
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="providerName">提供者名称</param>
        /// <param name="context">内容</param>
        /// <param name="defaultPort">默认端口</param>
        public ConnectionString(string name, string providerName, string context, int defaultPort)
        {
            Name = name;
            ProviderName = providerName;
            if (string.IsNullOrWhiteSpace(context)) return;

            string[] contexts = context.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in contexts)
            {
                if (string.IsNullOrWhiteSpace(item)) continue;
                string[] content = item.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                string key = content[0].Replace(" ", "");
                string value = null;
                if (content.Length > 1) value = string.Join("=", content, 1, content.Length - 1);

                if (key == "DataSource")
                {
                    DataSource = new DataSourceSet(value, defaultPort);
                    continue;
                }

                if (!Propertys.ContainsKey(key)) continue;

                Propertys[key].SetValue(this, value);
            }
        }
    }
}
