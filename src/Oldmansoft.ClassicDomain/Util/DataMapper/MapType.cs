using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    struct MapType
    {
        public readonly Type Source;

        public readonly Type Target;
        
        public MapType(Type source, Type target)
        {
            Source = source;
            Target = target;
        }

        public override int GetHashCode()
        {
            var source = Source.GetHashCode();
            return ((source << 5) + source) ^ Target.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is MapType)) return false;
            var other = (MapType)obj;
            return other.Source == Source && other.Target == Target;
        }

        public static bool operator ==(MapType left, MapType right)
        {
            return left.Source == right.Source && left.Target == right.Target;
        }

        public static bool operator !=(MapType left, MapType right)
        {
            return left.Source != right.Source || left.Target != right.Target;
        }
    }
}
