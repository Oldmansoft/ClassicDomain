using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    class MapStructProperty : MapProperty
    {
        public override void Map(object source, ref object target)
        {
            Setter.Set(target, Getter.Get(source));
        }
    }
}
