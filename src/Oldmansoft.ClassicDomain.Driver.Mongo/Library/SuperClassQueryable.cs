using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Library
{
    /// <summary>
    /// 用父类书写、用文档类型执行的查询。
    /// LINQ3 要求 lambda 参数类型与集合序列化器的 ValueType 相同。
    /// </summary>
    sealed class SuperClassQueryable<TDomain, TSuperDomain> : IOrderedQueryable<TSuperDomain>
        where TDomain : class, TSuperDomain
        where TSuperDomain : class
    {
        readonly SuperClassQueryProvider<TDomain, TSuperDomain> QueryProvider;

        public Type ElementType
        {
            get { return typeof(TSuperDomain); }
        }

        public Expression Expression { get; private set; }

        public IQueryProvider Provider
        {
            get { return QueryProvider; }
        }

        SuperClassQueryable(SuperClassQueryProvider<TDomain, TSuperDomain> provider, Expression expression)
        {
            QueryProvider = provider;
            Expression = expression;
        }

        public static IQueryable<TSuperDomain> Create(IQueryable<TDomain> source)
        {
            var provider = new SuperClassQueryProvider<TDomain, TSuperDomain>(source);
            var query = new SuperClassQueryable<TDomain, TSuperDomain>(provider, null);
            provider.Root = query;
            query.Expression = Expression.Constant(query);
            return query;
        }

        internal SuperClassQueryable<TDomain, TSuperDomain> Next(Expression expression)
        {
            return new SuperClassQueryable<TDomain, TSuperDomain>(QueryProvider, expression);
        }

        public IEnumerator<TSuperDomain> GetEnumerator()
        {
            return Provider.Execute<IEnumerable<TSuperDomain>>(Expression).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    sealed class SuperClassQueryProvider<TDomain, TSuperDomain> : IQueryProvider
        where TDomain : class, TSuperDomain
        where TSuperDomain : class
    {
        readonly IQueryable<TDomain> Source;

        public SuperClassQueryable<TDomain, TSuperDomain> Root { get; set; }

        public SuperClassQueryProvider(IQueryable<TDomain> source)
        {
            Source = source;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            var elementType = expression.Type.GetGenericArguments()[0];
            var method = GetType().GetMethods().Single(item => item.Name == "CreateQuery" && item.IsGenericMethodDefinition);
            return (IQueryable)method.MakeGenericMethod(elementType).Invoke(this, new object[] { expression });
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            if (typeof(TElement) == typeof(TSuperDomain))
            {
                return (IQueryable<TElement>)(object)Root.Next(expression);
            }
            return Source.Provider.CreateQuery<TElement>(Rewrite(expression));
        }

        public object Execute(Expression expression)
        {
            return Source.Provider.Execute(Rewrite(expression));
        }

        public TResult Execute<TResult>(Expression expression)
        {
            var rewritten = Rewrite(expression);
            if (typeof(TResult) == typeof(TSuperDomain))
            {
                return (TResult)(object)Source.Provider.Execute<TDomain>(rewritten);
            }
            if (typeof(TResult) == typeof(IEnumerable<TSuperDomain>))
            {
                // 驱动的 Execute 只翻译标量结果。返回序列时要 CreateQuery 后再枚举。
                var query = Source.Provider.CreateQuery<TDomain>(rewritten);
                return (TResult)(object)Enumerable.Cast<TSuperDomain>(query);
            }
            return Source.Provider.Execute<TResult>(rewritten);
        }

        Expression Rewrite(Expression expression)
        {
            return new SuperClassRewriter<TDomain, TSuperDomain>(Root, Source.Expression).Visit(expression);
        }
    }

    sealed class SuperClassRewriter<TDomain, TSuperDomain> : ExpressionVisitor
        where TDomain : class, TSuperDomain
        where TSuperDomain : class
    {
        readonly SuperClassQueryable<TDomain, TSuperDomain> Root;

        readonly Expression Source;

        readonly Dictionary<ParameterExpression, ParameterExpression> Parameters = new Dictionary<ParameterExpression, ParameterExpression>();

        public SuperClassRewriter(SuperClassQueryable<TDomain, TSuperDomain> root, Expression source)
        {
            Root = root;
            Source = source;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (ReferenceEquals(node.Value, Root)) return Source;
            return base.VisitConstant(node);
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType == ExpressionType.Quote)
            {
                return Expression.Quote(Visit(node.Operand));
            }
            return base.VisitUnary(node);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            var parameters = new ParameterExpression[node.Parameters.Count];
            for (var i = 0; i < parameters.Length; i++)
            {
                parameters[i] = (ParameterExpression)Visit(node.Parameters[i]);
            }
            return Expression.Lambda(Visit(node.Body), parameters);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (node.Type != typeof(TSuperDomain)) return base.VisitParameter(node);
            if (!Parameters.TryGetValue(node, out ParameterExpression mapped))
            {
                mapped = Expression.Parameter(typeof(TDomain), node.Name);
                Parameters.Add(node, mapped);
            }
            return mapped;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var method = node.Method;
            if (!method.IsGenericMethod || (method.DeclaringType != typeof(Queryable) && method.DeclaringType != typeof(Enumerable)))
            {
                return base.VisitMethodCall(node);
            }

            var arguments = method.GetGenericArguments();
            var changed = false;
            for (var i = 0; i < arguments.Length; i++)
            {
                if (arguments[i] != typeof(TSuperDomain)) continue;
                arguments[i] = typeof(TDomain);
                changed = true;
            }
            if (!changed) return base.VisitMethodCall(node);

            method = method.GetGenericMethodDefinition().MakeGenericMethod(arguments);
            var values = new Expression[node.Arguments.Count];
            for (var i = 0; i < values.Length; i++)
            {
                values[i] = Visit(node.Arguments[i]);
            }
            return node.Object == null
                ? Expression.Call(method, values)
                : Expression.Call(Visit(node.Object), method, values);
        }
    }
}
