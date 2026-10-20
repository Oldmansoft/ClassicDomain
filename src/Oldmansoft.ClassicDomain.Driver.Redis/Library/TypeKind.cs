using Oldmansoft.ClassicDomain.Util;
using System;
using System.Collections.Concurrent;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Library
{
    static class TypeKind
    {
        enum Kind
        {
            Scalar,
            ByteArray,
            List,
            Dictionary,
            NormalClass
        }

        static readonly ConcurrentDictionary<Type, Kind> Store = new ConcurrentDictionary<Type, Kind>();

        public static bool IsByteArray(Type type)
        {
            return Of(type) == Kind.ByteArray;
        }

        public static bool IsList(Type type)
        {
            return Of(type) == Kind.List;
        }

        public static bool IsDictionary(Type type)
        {
            return Of(type) == Kind.Dictionary;
        }

        public static bool IsNormalClass(Type type)
        {
            return Of(type) == Kind.NormalClass;
        }

        static Kind Of(Type type)
        {
            return Store.GetOrAdd(type, Classify);
        }

        static Kind Classify(Type type)
        {
            if (type == typeof(byte[])) return Kind.ByteArray;
            if (type.IsArrayOrGenericList()) return Kind.List;
            if (type.IsGenericDictionary()) return Kind.Dictionary;
            if (type.IsNormalClass()) return Kind.NormalClass;
            return Kind.Scalar;
        }
    }
}
