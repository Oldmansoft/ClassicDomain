using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Configuration
{
    /// <summary>
    /// 数据源集
    /// </summary>
    public class DataSourceSet : DataSource, IEnumerable<DataSource>
    {
        private int DefaultPort { get; set; }

        /// <summary>
        /// 源内容
        /// </summary>
        public string Origin { get; private set; }

        /// <summary>
        /// 数据源列表
        /// </summary>
        protected List<DataSource> List { get; private set; }

        /// <summary>
        /// 创建数据源集
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="defaultPort"></param>
        public DataSourceSet(string dataSource, int defaultPort)
            : base(defaultPort)
        {
            List = new List<DataSource>();
            Origin = dataSource;
            DefaultPort = defaultPort;

            string[] dataSources = dataSource.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < dataSources.Length; i++)
            {
                DataSource item = new DataSource(defaultPort);

                string[] addressContexts = dataSources[i].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (addressContexts.Length > 0)
                {
                    item.Host = addressContexts[0].Trim();
                }
                if (addressContexts.Length == 2)
                {
                    int port;
                    if (int.TryParse(addressContexts[1].Trim(), out port))
                    {
                        item.Port = port;
                    }
                }

                if (List.FirstOrDefault(o => o.Host == item.Host && o.Port == item.Port) == null)
                {
                    List.Add(item);
                }
            }

            if (List.Count > 0)
            {
                this.Host = List[0].Host;
                this.Port = List[0].Port;
            }
        }

        /// <summary>
        /// 获取主机列表
        /// </summary>
        /// <returns></returns>
        public string[] GetStrings()
        {
            string[] list = new string[List.Count];
            for (int i = 0; i < List.Count; i++)
            {
                list[i] = List[i].ToString();
            }
            return list;
        }

        /// <summary>
        /// 返回循环访问的枚举数
        /// </summary>
        /// <returns></returns>
        public IEnumerator<DataSource> GetEnumerator()
        {
            return List.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)List).GetEnumerator();
        }
    }
}
