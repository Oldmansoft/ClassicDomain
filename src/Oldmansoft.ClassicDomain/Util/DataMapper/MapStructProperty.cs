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
        public override void Map<TTarget>(string higherName, object source, ref TTarget target, MapConfig config)
        {
            TargetProperty.SetValue(target, SourceProperty.GetValue(source));
        }
    }
}
