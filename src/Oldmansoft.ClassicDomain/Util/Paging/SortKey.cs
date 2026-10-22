using System.Linq.Expressions;

namespace Oldmansoft.ClassicDomain.Util.Paging
{
    class SortKey
    {
        public LambdaExpression Selector { get; set; }

        public bool Descending { get; set; }
    }
}
