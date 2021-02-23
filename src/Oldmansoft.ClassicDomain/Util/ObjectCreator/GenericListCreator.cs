using System;
using System.Collections.Generic;

namespace Oldmansoft.ClassicDomain.Util
{
    class GenericListCreator : ICreator
    {
        private readonly Type ListType;

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
