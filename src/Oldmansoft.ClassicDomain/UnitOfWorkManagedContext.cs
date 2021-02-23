using Oldmansoft.ClassicDomain.Driver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Oldmansoft.ClassicDomain
{
    /// <summary>
    /// 工作单元管理项
    /// </summary>
    public abstract class UnitOfWorkManagedContext : IUnitOfWorkManagedItem
    {
        private static readonly ConcurrentDictionary<Type, string> ConnectionStringStore = new ConcurrentDictionary<Type, string>();

        /// <summary>
        /// 设置连接字符串
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="connectionString"></param>
        public static void SetConnectionString<TContext>(string connectionString)
        {
            ConnectionStringStore.AddOrUpdate(typeof(TContext), connectionString, (oldkey, oldvalue) => connectionString);
        }

        /// <summary>
        /// 获取连接字符串
        /// </summary>
        /// <param name="contextType"></param>
        /// <returns></returns>
        public static string GetConnectionString(Type contextType)
        {
            if (ConnectionStringStore.TryGetValue(contextType, out string result)) return result;
            throw new ClassicDomainException(contextType, string.Format("{0} 需要配置连接字符串，请使用 RepositoryFactory.SetConnectionString 静态方法。", contextType.FullName));
        }

        /// <summary>
        /// 更新命令集
        /// </summary>
        protected ConcurrentQueue<ICommand> Commands { get; private set; }

        void IUnitOfWorkManagedItem.Init(ConcurrentQueue<ICommand> commands)
        {
            Commands = commands;
        }

        /// <summary>
        /// 创建实体中，此方法由 UnitOfWork 调用
        /// </summary>
        void IUnitOfWorkManagedItem.ModelCreating()
        {
            OnModelCreating();
        }

        /// <summary>
        /// 在创建实体时
        /// </summary>
        protected abstract void OnModelCreating();
    }
}
