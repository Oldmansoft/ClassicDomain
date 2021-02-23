using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Oldmansoft.ClassicDomain.Util
{
    class NormalClassCreator : ICreator
    {
        readonly Func<object> Create;

        public NormalClassCreator(ConstructorInfo constructor)
        {
            var lambda = Expression.Lambda<Func<object>>(Expression.New(constructor));
            Create = lambda.Compile();
        }

        public object CreateObject()
        {
            return Create();
        }
    }
}
