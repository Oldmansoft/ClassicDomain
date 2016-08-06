using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        internal System.Linq.Expressions.Expression<Func<TDomain, TKey>> KeyExpression
        {
            get { return Store.KeyExpression; }
            set { Store.KeyExpression = value; }
        }

        /// <summary>
        /// 创建 In Process 实体集
        /// </summary>
        public DbSet()
        {
            List = new ChangeList<TDomain>();
        }

        /// <summary>
        /// 将添加
        /// </summary>
        /// <typeparam name="TDomain"></typeparam>
        /// <param name="domain"></param>
        public void WillAdd(TDomain domain)
        {
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
        /// 加载
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TDomain Load(TKey id)
        {
            return Store.Load(id);
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
