using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Oldmansoft.ClassicDomain.Driver.Mongo
{
    /// <summary>
    /// 实体集
    /// </summary>
    /// <typeparam name="TDomain"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IDbSet<TDomain, TKey> : IDbSet
    {
        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        IQueryable<TDomain> Query();

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TDomain Get(TKey id);

        /// <summary>
        /// 注册添加
        /// </summary>
        /// <param name="entity"></param>
        void RegisterAdd(TDomain entity);

        /// <summary>
        /// 注册替换
        /// </summary>
        /// <param name="entity"></param>
        void RegisterReplace(TDomain entity);

        /// <summary>
        /// 注册移除
        /// </summary>
        /// <param name="entity"></param>
        void RegisterRemove(TDomain entity);

        /// <summary>
        /// 注册执行
        /// </summary>
        /// <param name="execute"></param>
        void RegisterExecute(Func<MongoCollection<TDomain>, bool> execute);

        /// <summary>
        /// 获取 Mongo 集
        /// </summary>
        /// <returns></returns>
        MongoCollection<TDomain> GetCollection();
    }
}
