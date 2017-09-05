using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    /// <summary>
    /// 表达式助手
    /// </summary>
    public static class ExpressionHelper
    {
        /// <summary>
        /// 获取表达式中的属性
        /// </summary>
        /// <typeparam name="TEntity">类型</typeparam>
        /// <param name="source">表达式</param>
        /// <returns></returns>
        public static PropertyInfo GetProperty<TEntity>(this Expression<Func<TEntity, object>> source)
        {
            var member = source.Body;
            if (member.NodeType == ExpressionType.Convert && source.Body is UnaryExpression)
            {
                member = ((UnaryExpression)member).Operand;
            }
            if (!(member is MemberExpression)) return null;
            return ((MemberExpression)member).Member as PropertyInfo;
        }

        /// <summary>
        /// 获取表达式中的属性
        /// </summary>
        /// <typeparam name="TEntity">类型</typeparam>
        /// <typeparam name="TMember">成员</typeparam>
        /// <param name="source">表达式</param>
        /// <returns></returns>
        public static PropertyInfo GetProperty<TEntity, TMember>(this Expression<Func<TEntity, TMember>> source)
        {
            var member = source.Body;
            if (member.NodeType == ExpressionType.Convert && source.Body is UnaryExpression)
            {
                member = ((UnaryExpression)member).Operand;
            }
            if (!(member is MemberExpression)) return null;
            return ((MemberExpression)member).Member as PropertyInfo;
        }

        /// <summary>
        /// 获取表达式中的方法
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static MethodInfo GetMethod(this LambdaExpression source)
        {
            if (!(source.Body is UnaryExpression)) return null;
            var unaryExpression = (UnaryExpression)source.Body;

            if (!(unaryExpression.Operand is MethodCallExpression)) return null;
            var methodCallExpression = (MethodCallExpression)unaryExpression.Operand;

            if (!(methodCallExpression.Object is ConstantExpression)) return null;
            var constantExpression = (ConstantExpression)methodCallExpression.Object;

            if (!(constantExpression.Value is MethodInfo)) return null;
            return (MethodInfo)constantExpression.Value;
        }


        /// <summary>
        /// 获取属性表达式的内容
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string GetPropertyContent<TEntity>(this Expression<Func<TEntity, object>> expression)
        {
            var member = expression.Body;
            if (member.NodeType == ExpressionType.Convert && expression.Body is UnaryExpression)
            {
                member = ((UnaryExpression)member).Operand;
            }
            if (!(member is MemberExpression)) return null;

            var memberExpression = (MemberExpression)member;
            var content = memberExpression.Member.Name;
            while (memberExpression.Expression.GetType().Name == "PropertyExpression")
            {
                memberExpression = (MemberExpression)memberExpression.Expression;
                content = memberExpression.Member.Name + "." + content;
            }
            return content;
        }
    }
}
