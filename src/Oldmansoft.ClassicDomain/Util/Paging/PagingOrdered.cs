using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Oldmansoft.ClassicDomain.Util.Paging
{
    class PagingOrdered<TSource> : IPagingOrdered<TSource>
    {
        private IQueryable<TSource> Query { get; set; }

        private IOrderedQueryable<TSource> OrderedQuery { get; set; }

        private List<SortKey> Orders { get; set; }

        public PagingOrdered(IQueryable<TSource> query, IOrderedQueryable<TSource> orderedQuery, List<SortKey> orders)
        {
            Query = query;
            OrderedQuery = orderedQuery;
            Orders = orders;
        }

        public IPagingResult<TSource> Size(int value)
        {
            return new PagingResult<TSource>(Query, OrderedQuery, Orders, value);
        }

        public IPagingOrdered<TSource> ThenBy<TKey>(Expression<Func<TSource, TKey>> keySelector)
        {
            var orders = new List<SortKey>(Orders);
            orders.Add(new SortKey { Selector = keySelector, Descending = false });
            return new PagingOrdered<TSource>(Query, OrderedQuery.ThenBy(keySelector), orders);
        }

        public IPagingOrdered<TSource> ThenByDescending<TKey>(Expression<Func<TSource, TKey>> keySelector)
        {
            var orders = new List<SortKey>(Orders);
            orders.Add(new SortKey { Selector = keySelector, Descending = true });
            return new PagingOrdered<TSource>(Query, OrderedQuery.ThenByDescending(keySelector), orders);
        }
    }
}
