using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    abstract class MapContent : IMapContent
    {
        protected string PropertyName { get; private set; }

        protected Type SourceType { get; private set; }

        protected Type TargetType { get; private set; }
        
        protected IValueAction ValueAction { get; private set; }
        
        public IMapContent Init(string propertyName, Type sourceType, Type targetType, IValueAction valueAction)
        {
            PropertyName = propertyName;
            SourceType = sourceType;
            TargetType = targetType;
            ValueAction = valueAction;
            return this;
        }

        public abstract void Map<TTarget>(string higherName, object source, ref TTarget target, MapConfig config);
    }
}
