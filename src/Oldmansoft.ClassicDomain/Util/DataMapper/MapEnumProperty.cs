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
        public override void Map(string higherName,object source, ref object target, MapConfig config)
        {
            try
            {
                TargetProperty.SetValue(target, Enum.Parse(TargetType, SourceProperty.GetValue(source).ToString()));
            }
            catch (ArgumentException)
            {
            }
        }
    }
}
