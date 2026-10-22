using Oldmansoft.ClassicDomain.Util;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Library
{
    static class DiffEmitter
    {
        static readonly object Gate = new object();

        static readonly Dictionary<Type, Delegate> Diffs = new Dictionary<Type, Delegate>();

        static readonly Dictionary<Type, Delegate> Sames = new Dictionary<Type, Delegate>();

        static readonly HashSet<Type> DiffStack = new HashSet<Type>();

        static readonly HashSet<Type> SameStack = new HashSet<Type>();

        static readonly ModuleBuilder Module = CreateModule();

        static ModuleBuilder CreateModule()
        {
            var assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Oldmansoft.ClassicDomain.Driver.Mongo.DynamicDiff"), AssemblyBuilderAccess.Run);
            return assembly.DefineDynamicModule("DynamicDiff");
        }

        public static bool IsByteArray(Type type)
        {
            return type == typeof(byte[]);
        }

        public static bool IsList(Type type)
        {
            return !IsByteArray(type) && type.IsArrayOrGenericList();
        }

        public static bool IsDictionary(Type type)
        {
            return type.IsGenericDictionary();
        }

        public static bool IsClass(Type type)
        {
            return !IsByteArray(type) && !IsList(type) && !IsDictionary(type) && type.IsNormalClass();
        }

        public static Delegate Compile(Type type)
        {
            lock (Gate)
            {
                if (Diffs.TryGetValue(type, out Delegate existing)) return existing;
                if (!DiffStack.Add(type)) throw new NotSupportedException("类型 " + type.FullName + " 存在循环引用，无法生成比较器。");
                try
                {
                    var compiled = EmitDiff(type);
                    Diffs[type] = compiled;
                    return compiled;
                }
                finally
                {
                    DiffStack.Remove(type);
                }
            }
        }

        public static Delegate CompileSame(Type type)
        {
            lock (Gate)
            {
                if (Sames.TryGetValue(type, out Delegate existing)) return existing;
                if (!SameStack.Add(type)) throw new NotSupportedException("类型 " + type.FullName + " 存在循环引用，无法生成比较器。");
                try
                {
                    var compiled = EmitSame(type);
                    Sames[type] = compiled;
                    return compiled;
                }
                finally
                {
                    SameStack.Remove(type);
                }
            }
        }

        static Delegate EmitDiff(Type type)
        {
            PrepareChildren(type);
            var properties = Properties(type);
            var draft = Begin(type, "Diff", typeof(void), new[] { type, type, typeof(string), typeof(IUpdateBuffer) });
            var il = draft.Method.GetILGenerator();
            var path = il.DeclareLocal(typeof(string));
            foreach (var property in properties) EmitDiffProperty(il, property, path);
            il.Emit(OpCodes.Ret);
            return End(draft, typeof(Action<,,,>).MakeGenericType(type, type, typeof(string), typeof(IUpdateBuffer)));
        }

        static Delegate EmitSame(Type type)
        {
            PrepareChildren(type);
            var properties = Properties(type);
            var draft = Begin(type, "Same", typeof(bool), new[] { type, type });
            var il = draft.Method.GetILGenerator();
            var different = il.DefineLabel();
            foreach (var property in properties)
            {
                EmitLoadBoth(il, property);
                il.Emit(OpCodes.Call, SameMethod(property.PropertyType));
                il.Emit(OpCodes.Brfalse, different);
            }
            il.Emit(OpCodes.Ldc_I4_1);
            il.Emit(OpCodes.Ret);
            il.MarkLabel(different);
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Ret);
            return End(draft, typeof(Func<,,>).MakeGenericType(type, type, typeof(bool)));
        }

        sealed class Draft
        {
            public TypeBuilder Type;

            public MethodBuilder Method;
        }

        static Draft Begin(Type type, string name, Type returnType, Type[] parameters)
        {
            var typeBuilder = Module.DefineType(name + "_" + type.FullName.Replace('.', '_').Replace('+', '_') + "_" + type.MetadataToken, TypeAttributes.Public | TypeAttributes.Class);
            return new Draft
            {
                Type = typeBuilder,
                Method = typeBuilder.DefineMethod(name, MethodAttributes.Public | MethodAttributes.Static, returnType, parameters)
            };
        }

        static Delegate End(Draft draft, Type delegateType)
        {
            var created = draft.Type.CreateTypeInfo();
            return Delegate.CreateDelegate(delegateType, created.GetMethod(draft.Method.Name));
        }

        static void PrepareChildren(Type type)
        {
            foreach (var property in Properties(type))
            {
                var propertyType = property.PropertyType;
                if (IsClass(propertyType))
                {
                    Compile(propertyType);
                    CompileSame(propertyType);
                }
                else if (IsList(propertyType))
                {
                    var itemType = propertyType.GetEnumerableItemType();
                    if (IsClass(itemType))
                    {
                        Compile(itemType);
                        CompileSame(itemType);
                    }
                }
                else if (IsDictionary(propertyType))
                {
                    var valueType = propertyType.GetGenericArguments()[1];
                    if (IsClass(valueType))
                    {
                        Compile(valueType);
                        CompileSame(valueType);
                    }
                }
            }
        }

        static void EmitDiffProperty(ILGenerator il, PropertyInfo property, LocalBuilder path)
        {
            StorePath(il, path, property.Name);
            EmitLoadBoth(il, property);
            il.Emit(OpCodes.Ldloc, path);
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Call, DiffMethod(property.PropertyType));
        }

        static void EmitLoadBoth(ILGenerator il, PropertyInfo property)
        {
            var getter = property.GetGetMethod(true);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Callvirt, getter);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, getter);
        }

        static void StorePath(ILGenerator il, LocalBuilder path, string name)
        {
            var append = il.DefineLabel();
            var done = il.DefineLabel();
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Call, typeof(string).GetMethod("IsNullOrEmpty", new[] { typeof(string) }));
            il.Emit(OpCodes.Brfalse, append);
            il.Emit(OpCodes.Ldstr, name);
            il.Emit(OpCodes.Br, done);
            il.MarkLabel(append);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldstr, "." + name);
            il.Emit(OpCodes.Call, typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) }));
            il.MarkLabel(done);
            il.Emit(OpCodes.Stloc, path);
        }

        static MethodInfo DiffMethod(Type type)
        {
            if (IsByteArray(type)) return typeof(DiffOperations).GetMethod("DiffBytes");
            if (IsList(type)) return typeof(DiffOperations).GetMethod("DiffList").MakeGenericMethod(type.GetEnumerableItemType());
            if (IsDictionary(type))
            {
                var arguments = type.GetGenericArguments();
                return typeof(DiffOperations).GetMethod("DiffDictionary").MakeGenericMethod(arguments[0], arguments[1]);
            }
            if (IsClass(type)) return typeof(DiffOperations).GetMethod("DiffClass").MakeGenericMethod(type);
            return typeof(DiffOperations).GetMethod("DiffScalar").MakeGenericMethod(type);
        }

        static MethodInfo SameMethod(Type type)
        {
            if (IsByteArray(type)) return typeof(DiffOperations).GetMethod("SameBytes");
            if (IsList(type)) return typeof(DiffOperations).GetMethod("SameList").MakeGenericMethod(type.GetEnumerableItemType());
            if (IsDictionary(type))
            {
                var arguments = type.GetGenericArguments();
                return typeof(DiffOperations).GetMethod("SameDictionary").MakeGenericMethod(arguments[0], arguments[1]);
            }
            if (IsClass(type)) return typeof(DiffOperations).GetMethod("SameClass").MakeGenericMethod(type);
            return typeof(DiffOperations).GetMethod("SameScalar").MakeGenericMethod(type);
        }

        static PropertyInfo[] Properties(Type type)
        {
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var result = new List<PropertyInfo>();
            foreach (var property in properties)
            {
                if (!property.CanRead || property.GetIndexParameters().Length > 0) continue;
                result.Add(property);
            }
            return result.ToArray();
        }
    }
}
