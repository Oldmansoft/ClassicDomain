using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    class NormalClassCreator : ICreator
    {
        private ConstructorInfo Constructor;

        public NormalClassCreator(ConstructorInfo constructor)
        {
            Constructor = constructor;
        }

        public object CreateObject()
        {
            return Constructor.Invoke(new object[0]);
        }
    }
}
