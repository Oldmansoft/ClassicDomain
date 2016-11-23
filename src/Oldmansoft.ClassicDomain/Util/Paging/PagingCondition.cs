using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util.Paging
{
    class PagingCondition<TSource> : IPagingCondition<TSource>
    {
        private IQueryable<TSource> Query { get; set; }

        public PagingCondition(IQueryable<TSource> query)
        {
            Query = query;
        }

        public IPagingOrdered<TSource> OrderBy<TKey>(Expression<Func<TSource, TKey>> keySelector)
        {
            return new PagingOrdered<TSource>(Query, Query.OrderBy(keySelector));
        }

        public IPagingOrdered<TSource> OrderByDescending<TKey>(Expression<Func<TSource, TKey>> keySelector)
        {
            return new PagingOrdered<TSource>(Query, Query.OrderByDescending(keySelector));
        }

        public IPagingCondition<TSource> Where(Expression<Func<TSource, bool>> predicate)
        {
            return new PagingCondition<TSource>(Query.Where(predicate));
        }
    }
}
