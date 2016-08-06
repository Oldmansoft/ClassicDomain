using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Configuration
{
    /// <summary>
    /// 读取配置
    /// </summary>
    public class Config
    {
        /// <summary>
        /// 获取连接字符串
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="defaultPort">默认端口</param>
        /// <returns></returns>
        public static ConnectionString GetConnectionString(string name, int defaultPort)
        {
            var settings = ConfigurationManager.ConnectionStrings[name];
            if (settings == null)
            {
                throw new ConfigItemNotFoundException(string.Format("config 文件找不到配置项 {0}", name));
            }
            if (string.IsNullOrWhiteSpace(settings.ConnectionString))
            {
                throw new ConfigItemException(string.Format("config 文件的配置项 {0} ConnectionString 为空", name));
            }
            return new ConnectionString(name, settings.ProviderName, settings.ConnectionString, defaultPort);
        }

        /// <summary>
        /// 根据类别，继承式获取连接字符串
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <param name="defaultPort">默认端口</param>
        /// <returns></returns>
        public static ConnectionString GetConnectionString(Type type, int defaultPort)
        {
            string[] domains = type.FullName.Split('.');
            string currentKey = null;
            ConnectionStringSettings settings = null;
            for (int i = domains.Length - 1; i > -1; i--)
            {
                currentKey = string.Join(".", domains, 0, i + 1);
                settings = ConfigurationManager.ConnectionStrings[currentKey];
                if (settings != null)
                {
                    break;
                }
            }

            if (settings == null)
            {
                throw new ConfigItemNotFoundException(string.Format("config 文件找不到的配置项 {0}", type.FullName));
            }
            if (string.IsNullOrWhiteSpace(settings.ConnectionString))
            {
                throw new ConfigItemException(string.Format("config 文件的配置项 {0} ConnectionString 为空", type.FullName));
            }
            return new ConnectionString(currentKey, settings.ProviderName, settings.ConnectionString, defaultPort);
        }
    }
}
