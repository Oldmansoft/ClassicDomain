using System;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Core
{
    /// <summary>
    /// 实体集
    /// </summary>
    /// <typeparam name="TDomain"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IDbSet<TDomain, TKey> : IDbSet
    {
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TDomain Get(TKey id);

        /// <summary>
        /// 注册添加
        /// </summary>
        /// <param name="domain"></param>
        void RegisterAdd(TDomain domain);

        /// <summary>
        /// 注册替换
        /// </summary>
        /// <param name="domain"></param>
        void RegisterReplace(TDomain domain);

        /// <summary>
        /// 注册移除
        /// </summary>
        /// <param name="domain"></param>
        void RegisterRemove(TDomain domain);

        /// <summary>
        /// 注册执行
        /// </summary>
        /// <param name="execute"></param>
        void RegisterExecute(Func<StackExchange.Redis.IDatabase, bool> execute);

        /// <summary>
        /// 获取 Mongo 集
        /// </summary>
        /// <returns></returns>
        StackExchange.Redis.IDatabase GetDatabase();
    }
}
