using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.ClassicDomain.Util
{
    class CopyTargetModel
    {
        public string Name { get; set; }

        public TargetType Type { get; set; }
    }

    enum TargetType : short
    {
        Normal1,
        Importance1
    }
}
