using System;
using System.Collections.Generic;

namespace Oldmansoft.ClassicDomain.Util
{
    class GenericDictionaryCreator : ICreator
    {
        private readonly Type ListType;

        public GenericDictionaryCreator(Type type)
        {
            var args = type.GetGenericArguments();
            ListType = typeof(Dictionary<,>).MakeGenericType(args[0], args[1]);
        }

        public object CreateObject()
        {
            return Activator.CreateInstance(ListType);
        }
    }
}
