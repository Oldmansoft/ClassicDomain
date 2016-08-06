using System;
using System.Collections.Concurrent;

namespace Oldmansoft.ClassicDomain
{
    /// <summary>
    /// 连接名称获取主机名
    /// </summary>
    class ConnectionNameMapHost
    {
        private ConcurrentDictionary<string, string> HostMapping { get; set; }

        /// <summary>
        /// 创建
        /// </summary>
        public ConnectionNameMapHost()
        {
            HostMapping = new ConcurrentDictionary<string, string>();
        }

        /// <summary>
        /// 获取相应的主机设置
        /// 通过连接串名称缓存主机值
        /// </summary>
        /// <param name="uow"></param>
        /// <returns></returns>
        public string GetHost(IUnitOfWorkItem uow)
        {
            if (uow == null) throw new ArgumentNullException("uow");
            var name = uow.GetConnectionName();
            string result;
            if (HostMapping.TryGetValue(name, out result))
            {
                return result;
            }

            result = uow.GetHost();
            if (result == null) throw new ArgumentNullException(string.Format("{0}.GetHost()", uow.GetType().FullName), "不允许返回 null");
            HostMapping.TryAdd(name, result);
            return result;
        }
    }
}
