using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    abstract class MapContent : IMap
    {
        protected Type SourceType { get; private set; }

        protected Type TargetType { get; private set; }
        
        public virtual IMap Init(Type sourceType, Type targetType)
        {
            SourceType = sourceType;
            TargetType = targetType;
            return this;
        }

        public abstract void Map(object source, object target);
    }
}
