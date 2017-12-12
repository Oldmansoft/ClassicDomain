using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    class MapEnumProperty : MapProperty
    {
        public override void Map(object source, object target)
        {
            try
            {
                Setter.Set(target, Enum.ToObject(TargetPropertyType, Getter.Get(source)));
            }
            catch (ArgumentException)
            {
            }
        }
    }
}
