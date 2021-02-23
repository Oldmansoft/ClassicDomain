using System.Collections.Generic;
using System.Linq;

namespace Oldmansoft.ClassicDomain.Util.Paging
{
    class PagingResult<TSource> : IPagingResult<TSource>
    {
        private IQueryable<TSource> Query { get; set; }

        private IOrderedQueryable<TSource> OrderedQuery { get; set; }

        private int Size { get; set; }

        public PagingResult(IQueryable<TSource> query, IOrderedQueryable<TSource> orderedQuery, int size)
        {
            Query = query;
            OrderedQuery = orderedQuery;
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
    }
}
