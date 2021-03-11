using System;
using System.Collections.Concurrent;

namespace Oldmansoft.ClassicDomain
{
    /// <summary>
    /// 连接字符串
    /// </summary>
    public static class ConnectionString
    {
        private static readonly ConcurrentDictionary<Type, string> Store = new ConcurrentDictionary<Type, string>();

        /// <summary>
        /// 设置
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="connectionString"></param>
        public static void Set<TContext>(string connectionString)
            where TContext : UnitOfWorkManagedContext
        {
            Set(typeof(TContext), connectionString);
        }

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="contextType"></param>
        /// <param name="connectionString"></param>
        public static void Set(Type contextType, string connectionString)
        {
            Store.AddOrUpdate(contextType, connectionString, (oldkey, oldvalue) => connectionString);
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="contextType"></param>
        /// <returns></returns>
        public static string Get(Type contextType)
        {
            if (Store.TryGetValue(contextType, out string result)) return result;
            throw new ClassicDomainException(contextType, string.Format("{0} 需要配置连接字符串，请使用 ConnectionString.Set 静态方法。", contextType.FullName));
        }
    }
}
