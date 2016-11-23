using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public TSource[] GetResult(int number)
        {
            if (number < 1) number = 1;

            return OrderedQuery.Skip(Size * (number - 1)).Take(Size).ToArray();
        }

        public TSource[] GetResult(out int totalCount, int number)
        {
            totalCount = Query.Count();
            if (totalCount == 0) return new TSource[0];

            return GetResult(number);
        }
    }
}
