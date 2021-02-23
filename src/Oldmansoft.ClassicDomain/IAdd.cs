﻿namespace Oldmansoft.ClassicDomain
{
    /// <summary>
    /// 添加
    /// </summary>
    public interface IAdd<TDomain> where TDomain : class
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="domain"></param>
        void Add(TDomain domain);
    }
}