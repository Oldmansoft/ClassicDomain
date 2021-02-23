using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Oldmansoft.ClassicDomain.Driver.Mongo
{
    /// <summary>
    /// 表达式组
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class ExpressionGroup<TEntity>
    {
        internal List<Expression<Func<TEntity, object>>> Expressions { get; private set; }

        private ExpressionGroup()
        {
            Expressions = new List<Expression<Func<TEntity, object>>>();
        }

        /// <summary>
        /// 创建表达式组
        /// </summary>
        /// <param name="expression"></param>
        public ExpressionGroup(Expression<Func<TEntity, object>> expression)
        {
            Expressions = new List<Expression<Func<TEntity, object>>>();
            if (expression != null) Expressions.Add(expression);
        }

        /// <summary>
        /// 添加表达式
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public ExpressionGroup<TEntity> Add(Expression<Func<TEntity, object>> expression)
        {
            var other = new ExpressionGroup<TEntity>();
            other.Expressions.AddRange(Expressions);
            if (expression != null) other.Expressions.Add(expression);
            return other;
        }
    }
}
