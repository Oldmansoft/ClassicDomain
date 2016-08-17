using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Oldmansoft.ClassicDomain.Driver.Mongo
{
    /// <summary>
    /// 快速模式配置
    /// </summary>
    internal class FastModeConfig
    {
        private Dictionary<string, ConfigItem> Items { get; set; }

        /// <summary>
        /// 配置
        /// </summary>
        public FastModeConfig()
        {
            Items = new Dictionary<string, ConfigItem>();
        }

        private ConfigItem InitItem(string name)
        {
            var connectionString = Configuration.Config.GetConnectionString(name);
            var url = new MongoUrl(connectionString);
            var setting = MongoServerSettings.FromUrl(url);
            var uri = new Uri(connectionString);
            if (uri.GetDatabase() == string.Empty)
            {
                throw new ConfigItemException(string.Format("config 文件的配置项 {0} ConnectionString 需要指定数据库名称", name));
            }
            return new ConfigItem(CreateMongoServer(setting), setting.GetHost(), uri.GetDatabase());
        }

        /// <summary>
        /// 获取配置项
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ConfigItem Get(string name)
        {
            if (Items.ContainsKey(name))
            {
                return Items[name];
            }

            ConfigItem item = InitItem(name);
            lock (Items)
            {
                if (Items.ContainsKey(name))
                {
                    return Items[name];
                }

                Items.Add(name, item);
                return item;
            }
        }

        /// <summary>
        /// 创建服务器
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        protected virtual MongoServer CreateMongoServer(MongoServerSettings setting)
        {
            return new MongoServer(setting);
        }
    }
}
