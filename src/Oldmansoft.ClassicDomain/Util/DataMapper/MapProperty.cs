using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    abstract class MapProperty : IMap
    {
        protected IGetter Getter { get; private set; }

        protected ISetter Setter { get; private set; }

        protected Type SourcePropertyType { get; private set; }

        protected Type TargetPropertyType { get; private set; }
        
        public virtual IMap Init(Type sourceType, Type targetType, PropertyInfo sourceProperty, PropertyInfo targetProperty)
        {
            Getter = (IGetter)Activator.CreateInstance(typeof(GetterWrapper<,>).MakeGenericType(sourceType, sourceProperty.PropertyType), sourceProperty);
            Setter = (ISetter)Activator.CreateInstance(typeof(SetterWrapper<,>).MakeGenericType(targetType, targetProperty.PropertyType), targetProperty);
            SourcePropertyType = sourceProperty.PropertyType;
            TargetPropertyType = targetProperty.PropertyType;
            return this;
        }

        public abstract void Map(object source, object target);
    }

    interface ISetter
    {
        void Set(object caller, object value);
    }

    interface IGetter
    {
        object Get(object caller);
    }

    class SetterWrapper<TCaller, TValue> : ISetter
    {
        private Action<TCaller, TValue> Setter;

        public SetterWrapper(PropertyInfo property)
        {
            Setter = (Action<TCaller, TValue>)Delegate.CreateDelegate(typeof(Action<TCaller, TValue>), property.GetSetMethod(true));
        }
        
        void ISetter.Set(object caller, object value)
        {
            Setter((TCaller)caller, (TValue)value);
        }
    }

    class GetterWrapper<TCaller, TValue> : IGetter
    {
        private Func<TCaller, TValue> Getter;

        public GetterWrapper(PropertyInfo property)
        {
            Getter = (Func<TCaller, TValue>)Delegate.CreateDelegate(typeof(Func<TCaller, TValue>), property.GetGetMethod(true));
        }
        
        object IGetter.Get(object caller)
        {
            return Getter((TCaller)caller);
        }
    }
}
