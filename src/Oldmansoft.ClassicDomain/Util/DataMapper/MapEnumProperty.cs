using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    class MapEnumProperty : MapContentProperty
    {
        public override void Map(object source, ref object target)
        {
            try
            {
                TargetProperty.SetValue(target, Enum.ToObject(TargetType, SourceProperty.GetValue(source)));
            }
            catch (ArgumentException)
            {
            }
        }
    }
}
