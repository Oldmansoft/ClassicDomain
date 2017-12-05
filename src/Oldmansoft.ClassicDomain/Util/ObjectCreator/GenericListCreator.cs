using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    class GenericListCreator : ICreator
    {
        private Type ListType;

        public GenericListCreator(Type type)
        {
            ListType = typeof(List<>).MakeGenericType(type.GetGenericArguments()[0]);
        }

        public object CreateObject()
        {
            return Activator.CreateInstance(ListType);
        }
    }
}
