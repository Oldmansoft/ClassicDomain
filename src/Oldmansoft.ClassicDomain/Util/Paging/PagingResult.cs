using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Oldmansoft.ClassicDomain.Util.Paging
{
    class PagingResult<TSource> : IPagingResult<TSource>
    {
        static readonly MethodInfo OrderByMethod = QueryMethod("OrderBy");

        static readonly MethodInfo OrderByDescendingMethod = QueryMethod("OrderByDescending");

        static readonly MethodInfo ThenByMethod = QueryMethod("ThenBy");

        static readonly MethodInfo ThenByDescendingMethod = QueryMethod("ThenByDescending");

        private IQueryable<TSource> Query { get; set; }

        private IOrderedQueryable<TSource> OrderedQuery { get; set; }

        private List<SortKey> Orders { get; set; }

        private int Size { get; set; }

        public PagingResult(IQueryable<TSource> query, IOrderedQueryable<TSource> orderedQuery, List<SortKey> orders, int size)
        {
            Query = query;
            OrderedQuery = orderedQuery;
            Orders = orders;
            Size = size;
        }

        public IList<TSource> ToList(int number)
        {
            if (number < 1) number = 1;

            return OrderedQuery.Skip(Size * (number - 1)).Take(Size).ToList();
        }

        public IList<TSource> ToList(int number, out int totalCount)
        {
            totalCount = Query.Count();
            if (totalCount == 0) return new List<TSource>();

            return ToList(number);
        }

        public IList<TSource> ToListAfter(TSource last)
        {
            var filtered = last == null ? Query : Query.Where(After(last));
            IOrderedQueryable<TSource> ordered = null;
            foreach (var key in Orders)
            {
                var method = ordered == null
                    ? (key.Descending ? OrderByDescendingMethod : OrderByMethod)
                    : (key.Descending ? ThenByDescendingMethod : ThenByMethod);
                var closed = method.MakeGenericMethod(typeof(TSource), key.Selector.ReturnType);
                ordered = (IOrderedQueryable<TSource>)closed.Invoke(null, new object[] { ordered ?? (IQueryable<TSource>)filtered, key.Selector });
            }
            return ordered.Take(Size).ToList();
        }

        Expression<Func<TSource, bool>> After(TSource last)
        {
            var parameter = Expression.Parameter(typeof(TSource), "entity");
            Expression rest = null;
            for (var i = Orders.Count - 1; i >= 0; i--)
            {
                var key = Orders[i];
                var left = new ParameterReplace(key.Selector.Parameters[0], parameter).Visit(key.Selector.Body);
                var raw = key.Selector.Compile().DynamicInvoke(last);
                var right = Value(raw, left.Type);
                var after = Compare(left, right, !key.Descending);
                if (rest == null)
                {
                    rest = after;
                    continue;
                }

                var equalLeft = new ParameterReplace(key.Selector.Parameters[0], parameter).Visit(key.Selector.Body);
                var equal = Expression.Equal(equalLeft, Value(raw, equalLeft.Type));
                rest = Expression.OrElse(after, Expression.AndAlso(equal, rest));
            }
            return Expression.Lambda<Func<TSource, bool>>(rest, parameter);
        }

        static Expression Value(object raw, Type type)
        {
            if (raw == null || raw.GetType() == type) return Expression.Constant(raw, type);
            var underlying = Nullable.GetUnderlyingType(type);
            if (underlying != null && raw.GetType() == underlying) return Expression.Convert(Expression.Constant(raw), type);
            return Expression.Constant(raw, type);
        }

        static Expression Compare(Expression left, Expression right, bool greater)
        {
            if (UseOperator(left.Type))
            {
                return greater ? Expression.GreaterThan(left, right) : Expression.LessThan(left, right);
            }

            var method = left.Type.GetMethod("CompareTo", new[] { left.Type }) ?? left.Type.GetMethod("CompareTo", new[] { typeof(object) });
            if (method == null) throw new NotSupportedException("排序键不支持继续取下一页：" + left.Type.FullName);
            var compared = Expression.Call(left, method, method.GetParameters()[0].ParameterType == typeof(object) ? (Expression)Expression.Convert(right, typeof(object)) : right);
            return greater ? Expression.GreaterThan(compared, Expression.Constant(0)) : Expression.LessThan(compared, Expression.Constant(0));
        }

        static bool UseOperator(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.DateTime:
                    return true;
            }
            return type == typeof(DateTimeOffset);
        }

        static MethodInfo QueryMethod(string name)
        {
            return typeof(Queryable).GetMethods().First(method => method.Name == name && method.GetParameters().Length == 2);
        }

        sealed class ParameterReplace : ExpressionVisitor
        {
            readonly ParameterExpression From;

            readonly ParameterExpression To;

            public ParameterReplace(ParameterExpression from, ParameterExpression to)
            {
                From = from;
                To = to;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == From ? To : base.VisitParameter(node);
            }
        }
    }
}
