using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    class EmptyCreator : ICreator
    {
        public static readonly EmptyCreator Instance = new EmptyCreator();

        private EmptyCreator() { }

        public object CreateObject()
        {
            return null;
        }
    }
}
