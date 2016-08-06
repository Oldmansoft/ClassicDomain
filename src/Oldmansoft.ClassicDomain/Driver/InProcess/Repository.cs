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
    public class Repository<TDomain, TKey> : IRepositoryLoad<TDomain, TKey> where TDomain : class
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
        /// 加载
        /// </summary>
        /// <returns></returns>
        public TDomain Load(TKey id)
        {
            return Context.Set<TDomain, TKey>().Load(id);
        }

        /// <summary>
        /// 加载多个实体
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public TDomain[] Load(TKey[] ids)
        {
            var result = new TDomain[ids.Length];
            var context = Context.Set<TDomain, TKey>();
            for (var i = 0; i < ids.Length; i++)
            {
                result[i] = context.Load(ids[i]);
            }
            return result;
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
