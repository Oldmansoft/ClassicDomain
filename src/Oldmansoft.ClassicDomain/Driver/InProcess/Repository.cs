using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.InProcess
{
    /// <summary>
    /// 进程内仓储
    /// </summary>
    /// <typeparam name="TDomain"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class Repository<TDomain, TKey> : IRepositoryGet<TDomain, TKey> where TDomain : class
    {
        private Context Context { get; set; }

        /// <summary>
        /// 创建 In Process 仓储库
        /// </summary>
        /// <param name="context"></param>
        public Repository(Context context)
        {
            Context = context;
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        public TDomain Get(TKey id)
        {
            return Context.Set<TDomain, TKey>().Get(id);
        }
        
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="domain"></param>
        public void Add(TDomain domain)
        {
            Context.Set<TDomain, TKey>().WillAdd(domain);
        }

        /// <summary>
        /// 替换
        /// </summary>
        /// <param name="domain"></param>
        public void Replace(TDomain domain)
        {
            Context.Set<TDomain, TKey>().WillReplace(domain);
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="domain"></param>
        public void Remove(TDomain domain)
        {
            Context.Set<TDomain, TKey>().WillRemove(domain);
        }
    }
}
