using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oldmansoft.ClassicDomain.Util;

namespace Oldmansoft.ClassicDomain.Driver.InProcess
{
    class DbSet<TDomain, TKey> : IDbSet
    {
        private static StoreManager<TDomain, TKey> Store;

        static DbSet()
        {
            Store = new StoreManager<TDomain, TKey>();
        }

        private System.Collections.Concurrent.ConcurrentQueue<ICommand> Commands { get; set; }
        
        /// <summary>
        /// 主键表达式
        /// </summary>
        private System.Linq.Expressions.Expression<Func<TDomain, TKey>> KeyExpression
        {
            get { return Store.KeyExpression; }
            set { Store.KeyExpression = value; }
        }

        /// <summary>
        /// 主键获取
        /// </summary>
        private Func<TDomain, TKey> KeyExpressionCompile { get; set; }

        /// <summary>
        /// 属性设值器
        /// </summary>
        private ISetter PropertySetter { get; set; }

        /// <summary>
        /// 创建 In Process 实体集
        /// </summary>
        /// <param name="keyExpression"></param>
        public DbSet(System.Linq.Expressions.Expression<Func<TDomain, TKey>> keyExpression)
        {
            Commands = new System.Collections.Concurrent.ConcurrentQueue<ICommand>();
            KeyExpression = keyExpression;
            KeyExpressionCompile = KeyExpression.Compile();
            if (typeof(TKey) == typeof(Guid))
            {
                PropertySetter = new PropertySetter<TDomain, TKey>(KeyExpression.GetProperty());
            }
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
                    PropertySetter.Set(domain, GuidGenerator.Default.Create(StorageMapping.MemoryMapping));
                }
            }
        }

        /// <summary>
        /// 将添加
        /// </summary>
        /// <param name="domain"></param>
        public void RegisterAdd(TDomain domain)
        {
            TrySetDomainKey(domain);
            domain = domain.MapTo<TDomain>();
            Commands.Enqueue(new Commands.AddCommand<TDomain, TKey>(Store, domain));
        }

        /// <summary>
        /// 将替换
        /// </summary>
        /// <param name="domain"></param>
        public void RegisterReplace(TDomain domain)
        {
            domain = domain.MapTo<TDomain>();
            Commands.Enqueue(new Commands.ReplaceCommand<TDomain, TKey>(Store, domain));
        }

        /// <summary>
        /// 将移除
        /// </summary>
        /// <param name="domain"></param>
        public void RegisterRemove(TDomain domain)
        {
            domain = domain.MapTo<TDomain>();
            Commands.Enqueue(new Commands.RemoveCommand<TDomain, TKey>(Store, domain));
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TDomain Get(TKey id)
        {
            return Store.Get(id);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        public IQueryable<TDomain> Query()
        {
            return Store.Query();
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <returns></returns>
        public int Commit()
        {
            int result = 0;
            ICommand command;
            while (Commands.TryDequeue(out command))
            {
                if (command.Execute()) result++;
            }
            return result;
        }
    }
}
