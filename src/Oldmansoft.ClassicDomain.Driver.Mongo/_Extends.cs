using Oldmansoft.ClassicDomain.Driver.Mongo;
using System;
using System.Linq.Expressions;

namespace Oldmansoft.ClassicDomain
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class Extends
    {
        internal static string GetDatabaseName(this Uri source)
        {
            return source.AbsolutePath.Substring(1);
        }

        /// <summary>
        /// 创建表达式组
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static ExpressionGroup<TEntity> CreateGroup<TEntity>(this TEntity entity, Expression<Func<TEntity, object>> expression)
        {
            return new ExpressionGroup<TEntity>(expression);
        }
    }
}
