using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Configuration
{
    /// <summary>
    /// 数据源
    /// </summary>
    public class DataSource
    {
        /// <summary>
        /// 主机
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 创建数据源
        /// </summary>
        /// <param name="port"></param>
        internal DataSource(int port)
        {
            Host = "localhost";
            Port = port;
        }

        /// <summary>
        /// 生成主机连接串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}:{1}", Host, Port);
        }
    }
}
