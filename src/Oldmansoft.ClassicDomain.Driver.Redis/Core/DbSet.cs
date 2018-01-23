using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
using Oldmansoft.ClassicDomain.Util;
using System.Linq.Expressions;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Core
{
    /// <summary>
    /// 实体集
    /// </summary>
    /// <typeparam name="TDomain"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    internal abstract class DbSet<TDomain, TKey> : IDbSet<TDomain, TKey>
    {
        /// <summary>
        /// 领域名称
        /// </summary>
        protected string DomainName { get; private set; }

        protected ConcurrentQueue<ICommand> Commands { get; private set; }
        
        private Expression<Func<TDomain, TKey>> KeyExpression { get; set; }

        /// <summary>
        /// 主键表达式
        /// </summary>
        protected Func<TDomain, TKey> KeyExpressionCompile { get; private set; }

        /// <summary>
        /// 属性设值器
        /// </summary>
        private ISetter PropertySetter { get; set; }

        /// <summary>
        /// 配置
        /// </summary>
        protected ConfigItem Config { get; private set; }

        /// <summary>
        /// 数据库
        /// </summary>
        protected IDatabase Db { get; private set; }

        /// <summary>
        /// 创建实体集
        /// </summary>
        /// <param name="config"></param>
        /// <param name="db"></param>
        /// <param name="keyExpression"></param>
        public DbSet(ConfigItem config, IDatabase db, Expression<Func<TDomain, TKey>> keyExpression)
        {
            DomainName = typeof(TDomain).FullName;
            Commands = new ConcurrentQueue<ICommand>();
            Config = config;
            Db = db;
            KeyExpression = keyExpression;
            KeyExpressionCompile = keyExpression.Compile();
            if (typeof(TKey) == typeof(Guid))
            {
                PropertySetter = new PropertySetter<TDomain, TKey>(KeyExpression.GetProperty());
            }
        }

        /// <summary>
        /// 拼接 Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string MergeKey(TKey key)
        {
            string id;
            if (key is Guid) id = ((Guid)Convert.ChangeType(key, typeof(Guid))).ToString("N");
            else id = key.ToString();

            return string.Format("{0}:{1}", DomainName, id);
        }

        /// <summary>
        /// 获取键值
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        protected string GetKey(TDomain domain)
        {
            return MergeKey(KeyExpressionCompile(domain));
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public abstract TDomain Get(TKey id);

        /// <summary>
        /// 注册添加
        /// </summary>
        /// <param name="domain"></param>
        public abstract void RegisterAdd(TDomain domain);

        /// <summary>
        /// 注册替换
        /// </summary>
        /// <param name="domain"></param>
        public abstract void RegisterReplace(TDomain domain);

        /// <summary>
        /// 注册移除
        /// </summary>
        /// <param name="domain"></param>
        public abstract void RegisterRemove(TDomain domain);

        /// <summary>
        /// 提交
        /// </summary>
        /// <returns></returns>
        public virtual int Commit()
        {
            int result = 0;
            ICommand command;
            while(Commands.TryDequeue(out command))
            {
                if (command.Execute()) result++;
            }
            return result;
        }

        /// <summary>
        /// 注册执行
        /// </summary>
        /// <param name="execute"></param>
        public void RegisterExecute(Func<IDatabase, bool> execute)
        {
            Commands.Enqueue(new Commands.ActionCommand<TDomain>(Db, execute));
        }

        /// <summary>
        /// 获取数据库
        /// </summary>
        /// <returns></returns>
        public IDatabase GetDatabase()
        {
            return Db;
        }

        /// <summary>
        /// 是否低服务器版本
        /// </summary>
        /// <returns></returns>
        public bool IsLowServerVersion()
        {
            return Config.IsLowServerVersion;
        }

        /// <summary>
        /// 尝试设置主键
        /// </summary>
        /// <param name="domain"></param>
        protected void TrySetDomainKey(TDomain domain)
        {
            if (PropertySetter != null)
            {
                if ((Guid)(object)KeyExpressionCompile(domain) == Guid.Empty)
                {
                    PropertySetter.Set(domain, Guid.NewGuid());
                }
            }
        }
    }
}
