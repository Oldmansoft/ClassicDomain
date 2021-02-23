using System;
using System.Linq;

namespace Oldmansoft.ClassicDomain.Util.Paging
{
    class PagingOrdered<TSource> : IPagingOrdered<TSource>
    {
        private IQueryable<TSource> Query { get; set; }

        private IOrderedQueryable<TSource> OrderedQuery { get; set; }

        public PagingOrdered(IQueryable<TSource> query, IOrderedQueryable<TSource> orderedQuery)
        {
            Query = query;
            OrderedQuery = orderedQuery;
        }

        public IPagingResult<TSource> Size(int value)
        {
            return new PagingResult<TSource>(Query, OrderedQuery, value);
        }

        public IPagingOrdered<TSource> ThenBy<TKey>(System.Linq.Expressions.Expression<Func<TSource, TKey>> keySelector)
        {
            return new PagingOrdered<TSource>(Query, OrderedQuery.ThenBy(keySelector));
        }

        public IPagingOrdered<TSource> ThenByDescending<TKey>(System.Linq.Expressions.Expression<Func<TSource, TKey>> keySelector)
        {
            return new PagingOrdered<TSource>(Query, OrderedQuery.ThenByDescending(keySelector));
        }
    }
}
