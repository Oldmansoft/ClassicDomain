using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    class MapStructProperty : MapContentProperty
    {
        public override void Map(object source, ref object target)
        {
            TargetProperty.SetValue(target, SourceProperty.GetValue(source));
        }
    }
}
