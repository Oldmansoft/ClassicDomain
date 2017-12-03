using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    class MapEnum : MapContent
    {
        public override void Map<TTarget>(string higherName,object source, ref TTarget target, MapConfig config)
        {
            try
            {
                ValueAction.SetValue(ref target, Enum.Parse(TargetType, ValueAction.GetValue(source).ToString()));
            }
            catch (ArgumentException)
            {
            }
        }
    }
}
