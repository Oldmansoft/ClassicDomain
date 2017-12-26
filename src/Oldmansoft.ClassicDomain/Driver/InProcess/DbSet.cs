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

        private ChangeList<TDomain> List { get; set; }

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
            List = new ChangeList<TDomain>();
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
        public void WillAdd(TDomain domain)
        {
            TrySetDomainKey(domain);
            List.Addeds.Enqueue(domain);
        }

        /// <summary>
        /// 将替换
        /// </summary>
        /// <param name="domain"></param>
        public void WillReplace(TDomain domain)
        {
            List.Updateds.Enqueue(domain);
        }

        /// <summary>
        /// 将移除
        /// </summary>
        /// <param name="domain"></param>
        public void WillRemove(TDomain domain)
        {
            List.Deleteds.Enqueue(domain);
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
            TDomain domain;
            while (List.Addeds.TryDequeue(out domain))
            {
                if (Store.Add(domain)) result++;
            }
            while (List.Updateds.TryDequeue(out domain))
            {
                if (Store.Replace(domain)) result++;
            }
            while (List.Deleteds.TryDequeue(out domain))
            {
                if (Store.Remove(domain)) result++;
            }
            return result;
        }
    }
}
