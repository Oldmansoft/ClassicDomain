using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    class NormalClassCreator : ICreator
    {
        Func<object> Create;

        public NormalClassCreator(Type type, ConstructorInfo constructor)
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
