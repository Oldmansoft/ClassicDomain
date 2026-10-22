using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Library
{
    static class RedisEmitter
    {
        static readonly object Gate = new object();

        static readonly Dictionary<Type, Delegate> Diffs = new Dictionary<Type, Delegate>();

        static readonly Dictionary<Type, Delegate> Writes = new Dictionary<Type, Delegate>();

        static readonly Dictionary<Type, Delegate> Reads = new Dictionary<Type, Delegate>();

        static readonly HashSet<Type> DiffStack = new HashSet<Type>();

        static readonly HashSet<Type> WriteStack = new HashSet<Type>();

        static readonly HashSet<Type> ReadStack = new HashSet<Type>();

        static readonly ModuleBuilder Module = CreateModule();

        static readonly MethodInfo JoinMethod = typeof(RedisAccess).GetMethod("Join");

        static readonly MethodInfo DiffScalarMethod = typeof(RedisAccess).GetMethod("DiffScalar");

        static readonly MethodInfo DiffBytesMethod = typeof(RedisAccess).GetMethod("DiffBytes");

        static readonly MethodInfo DiffClassMarkerMethod = typeof(RedisAccess).GetMethod("DiffClassMarker");

        static readonly MethodInfo DiffListMethod = typeof(RedisAccess).GetMethod("DiffList", new[] { typeof(UpdatedCommand), typeof(string), typeof(object), typeof(object), typeof(RuntimeTypeHandle) });

        static readonly MethodInfo DiffDictionaryMethod = typeof(RedisAccess).GetMethod("DiffDictionary", new[] { typeof(UpdatedCommand), typeof(string), typeof(object), typeof(object), typeof(RuntimeTypeHandle) });

        static readonly MethodInfo WriteScalarMethod = typeof(RedisAccess).GetMethod("WriteScalar");

        static readonly MethodInfo WriteBytesMethod = typeof(RedisAccess).GetMethod("WriteBytes");

        static readonly MethodInfo WriteClassMarkerMethod = typeof(RedisAccess).GetMethod("WriteClassMarker");

        static readonly MethodInfo WriteListMethod = typeof(RedisAccess).GetMethod("WriteList");

        static readonly MethodInfo WriteDictionaryMethod = typeof(RedisAccess).GetMethod("WriteDictionary");

        static readonly MethodInfo HasFieldMethod = typeof(RedisAccess).GetMethod("HasField");

        static readonly MethodInfo FieldMethod = typeof(RedisAccess).GetMethod("Field");

        static readonly MethodInfo ParseMethod = typeof(RedisAccess).GetMethod("Parse");

        static readonly MethodInfo CreateMethod = typeof(RedisAccess).GetMethod("Create");

        static readonly MethodInfo ReadBytesMethod = typeof(RedisAccess).GetMethod("ReadBytes");

        static readonly MethodInfo ReadListMethod = typeof(RedisAccess).GetMethod("ReadList");

        static readonly MethodInfo ReadDictionaryMethod = typeof(RedisAccess).GetMethod("ReadDictionary");

        static ModuleBuilder CreateModule()
        {
            var assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Oldmansoft.ClassicDomain.Driver.Redis.DynamicAccess"), AssemblyBuilderAccess.Run);
            return assembly.DefineDynamicModule("DynamicAccess");
        }

        public static Delegate CompileDiff(Type type)
        {
            return Compile(type, Diffs, DiffStack, EmitDiff);
        }

        public static Delegate CompileWrite(Type type)
        {
            return Compile(type, Writes, WriteStack, EmitWrite);
        }

        public static Delegate CompileRead(Type type)
        {
            return Compile(type, Reads, ReadStack, EmitRead);
        }

        static Delegate Compile(Type type, Dictionary<Type, Delegate> cache, HashSet<Type> stack, Func<Type, Delegate> emit)
        {
            lock (Gate)
            {
                Delegate existing;
                if (cache.TryGetValue(type, out existing)) return existing;
                if (!stack.Add(type)) throw new NotSupportedException("类型 " + type.FullName + " 存在循环引用，无法生成访问器。");
                try
                {
                    var compiled = emit(type);
                    cache[type] = compiled;
                    return compiled;
                }
                finally
                {
                    stack.Remove(type);
                }
            }
        }

        static Delegate EmitDiff(Type type)
        {
            var properties = Properties(type);
            foreach (var property in properties)
            {
                if (TypeKind.IsNormalClass(property.PropertyType)) CompileDiff(property.PropertyType);
            }
            var draft = Begin(type, "Diff", typeof(void), new[] { typeof(object), typeof(object), typeof(UpdatedCommand), typeof(string) });
            var il = draft.Method.GetILGenerator();
            var path = il.DeclareLocal(typeof(string));
            foreach (var property in properties) EmitDiffProperty(il, property, path);
            il.Emit(OpCodes.Ret);
            return End(draft, typeof(Action<,,,>).MakeGenericType(typeof(object), typeof(object), typeof(UpdatedCommand), typeof(string)));
        }

        static Delegate EmitWrite(Type type)
        {
            var properties = Properties(type);
            foreach (var property in properties)
            {
                if (TypeKind.IsNormalClass(property.PropertyType)) CompileWrite(property.PropertyType);
            }
            var draft = Begin(type, "Write", typeof(void), new[] { typeof(object), typeof(UpdatedCommand), typeof(string) });
            var il = draft.Method.GetILGenerator();
            var path = il.DeclareLocal(typeof(string));
            foreach (var property in properties) EmitWriteProperty(il, property, path);
            il.Emit(OpCodes.Ret);
            return End(draft, typeof(Action<,,>).MakeGenericType(typeof(object), typeof(UpdatedCommand), typeof(string)));
        }

        static Delegate EmitRead(Type type)
        {
            var properties = Properties(type);
            foreach (var property in properties)
            {
                if (TypeKind.IsNormalClass(property.PropertyType)) CompileRead(property.PropertyType);
            }
            var draft = Begin(type, "Read", typeof(void), new[] { typeof(object), typeof(DataGetMapping), typeof(string) });
            var il = draft.Method.GetILGenerator();
            var path = il.DeclareLocal(typeof(string));
            foreach (var property in properties) EmitReadProperty(il, property, path);
            il.Emit(OpCodes.Ret);
            return End(draft, typeof(Action<,,>).MakeGenericType(typeof(object), typeof(DataGetMapping), typeof(string)));
        }

        static void EmitDiffProperty(ILGenerator il, PropertyInfo property, LocalBuilder path)
        {
            var propertyType = property.PropertyType;
            EmitPath(il, path, 3, property.Name);
            if (TypeKind.IsByteArray(propertyType))
            {
                PushResultAndPath(il, path, 2);
                EmitValue(il, property, 0);
                EmitValue(il, property, 1);
                il.Emit(OpCodes.Call, DiffBytesMethod);
                return;
            }
            if (TypeKind.IsList(propertyType))
            {
                PushResultAndPath(il, path, 2);
                EmitValue(il, property, 0);
                EmitValue(il, property, 1);
                il.Emit(OpCodes.Ldtoken, propertyType);
                il.Emit(OpCodes.Call, DiffListMethod);
                return;
            }
            if (TypeKind.IsDictionary(propertyType))
            {
                PushResultAndPath(il, path, 2);
                EmitValue(il, property, 0);
                EmitValue(il, property, 1);
                il.Emit(OpCodes.Ldtoken, propertyType);
                il.Emit(OpCodes.Call, DiffDictionaryMethod);
                return;
            }
            if (TypeKind.IsNormalClass(propertyType))
            {
                PushResultAndPath(il, path, 2);
                EmitValue(il, property, 0);
                EmitValue(il, property, 1);
                il.Emit(OpCodes.Ldstr, propertyType.FullName);
                il.Emit(OpCodes.Call, DiffClassMarkerMethod);
                var child = (Action<object, object, UpdatedCommand, string>)CompileDiff(propertyType);
                EmitValue(il, property, 0);
                EmitValue(il, property, 1);
                il.Emit(OpCodes.Ldarg_2);
                il.Emit(OpCodes.Ldloc, path);
                il.Emit(OpCodes.Call, child.Method);
                return;
            }
            PushResultAndPath(il, path, 2);
            EmitValue(il, property, 0);
            EmitValue(il, property, 1);
            il.Emit(OpCodes.Call, DiffScalarMethod);
        }

        static void EmitWriteProperty(ILGenerator il, PropertyInfo property, LocalBuilder path)
        {
            var propertyType = property.PropertyType;
            EmitPath(il, path, 2, property.Name);
            if (TypeKind.IsNormalClass(propertyType))
            {
                var local = il.DeclareLocal(typeof(object));
                var skip = il.DefineLabel();
                EmitValue(il, property, 0);
                il.Emit(OpCodes.Stloc, local);
                il.Emit(OpCodes.Ldloc, local);
                il.Emit(OpCodes.Brfalse, skip);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldloc, path);
                il.Emit(OpCodes.Ldstr, propertyType.FullName);
                il.Emit(OpCodes.Call, WriteClassMarkerMethod);
                var child = (Action<object, UpdatedCommand, string>)CompileWrite(propertyType);
                il.Emit(OpCodes.Ldloc, local);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldloc, path);
                il.Emit(OpCodes.Call, child.Method);
                il.MarkLabel(skip);
                return;
            }

            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldloc, path);
            EmitValue(il, property, 0);
            if (TypeKind.IsByteArray(propertyType)) il.Emit(OpCodes.Call, WriteBytesMethod);
            else if (TypeKind.IsList(propertyType))
            {
                il.Emit(OpCodes.Ldtoken, propertyType);
                il.Emit(OpCodes.Call, WriteListMethod);
            }
            else if (TypeKind.IsDictionary(propertyType))
            {
                il.Emit(OpCodes.Ldtoken, propertyType);
                il.Emit(OpCodes.Call, WriteDictionaryMethod);
            }
            else il.Emit(OpCodes.Call, WriteScalarMethod);
        }

        static void EmitReadProperty(ILGenerator il, PropertyInfo property, LocalBuilder path)
        {
            var propertyType = property.PropertyType;
            EmitPath(il, path, 2, property.Name);
            if (TypeKind.IsByteArray(propertyType))
            {
                EmitAssign(il, property, path, ReadBytesMethod, false);
                return;
            }
            if (TypeKind.IsList(propertyType))
            {
                EmitAssign(il, property, path, ReadListMethod, true);
                return;
            }
            if (TypeKind.IsDictionary(propertyType))
            {
                EmitAssign(il, property, path, ReadDictionaryMethod, true);
                return;
            }
            if (TypeKind.IsNormalClass(propertyType))
            {
                var skip = il.DefineLabel();
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldloc, path);
                il.Emit(OpCodes.Call, HasFieldMethod);
                il.Emit(OpCodes.Brfalse, skip);
                var childValue = il.DeclareLocal(typeof(object));
                il.Emit(OpCodes.Ldtoken, propertyType);
                il.Emit(OpCodes.Call, CreateMethod);
                il.Emit(OpCodes.Stloc, childValue);
                var child = (Action<object, DataGetMapping, string>)CompileRead(propertyType);
                il.Emit(OpCodes.Ldloc, childValue);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldloc, path);
                il.Emit(OpCodes.Call, child.Method);
                Store(il, property, childValue);
                il.MarkLabel(skip);
                return;
            }

            var absent = il.DefineLabel();
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldloc, path);
            il.Emit(OpCodes.Call, HasFieldMethod);
            il.Emit(OpCodes.Brfalse, absent);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Castclass, property.DeclaringType);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldloc, path);
            il.Emit(OpCodes.Call, FieldMethod);
            il.Emit(OpCodes.Ldtoken, propertyType);
            il.Emit(OpCodes.Call, ParseMethod);
            if (propertyType.IsValueType) il.Emit(OpCodes.Unbox_Any, propertyType);
            else if (propertyType != typeof(object)) il.Emit(OpCodes.Castclass, propertyType);
            il.Emit(OpCodes.Callvirt, property.GetSetMethod());
            il.MarkLabel(absent);
        }

        static void EmitAssign(ILGenerator il, PropertyInfo property, LocalBuilder path, MethodInfo read, bool withType)
        {
            var local = il.DeclareLocal(read.ReturnType);
            var skip = il.DefineLabel();
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldloc, path);
            if (withType) il.Emit(OpCodes.Ldtoken, property.PropertyType);
            il.Emit(OpCodes.Call, read);
            il.Emit(OpCodes.Stloc, local);
            il.Emit(OpCodes.Ldloc, local);
            il.Emit(OpCodes.Brfalse, skip);
            Store(il, property, local);
            il.MarkLabel(skip);
        }

        static void Store(ILGenerator il, PropertyInfo property, LocalBuilder value)
        {
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Castclass, property.DeclaringType);
            il.Emit(OpCodes.Ldloc, value);
            if (property.PropertyType.IsValueType) il.Emit(OpCodes.Unbox_Any, property.PropertyType);
            else if (value.LocalType != property.PropertyType) il.Emit(OpCodes.Castclass, property.PropertyType);
            il.Emit(OpCodes.Callvirt, property.GetSetMethod());
        }

        static void PushResultAndPath(ILGenerator il, LocalBuilder path, int resultArg)
        {
            Ldarg(il, resultArg);
            il.Emit(OpCodes.Ldloc, path);
        }

        static void EmitPath(ILGenerator il, LocalBuilder path, int prefixArg, string name)
        {
            Ldarg(il, prefixArg);
            il.Emit(OpCodes.Ldstr, name);
            il.Emit(OpCodes.Call, JoinMethod);
            il.Emit(OpCodes.Stloc, path);
        }

        static void EmitValue(ILGenerator il, PropertyInfo property, int instanceArg)
        {
            var done = il.DefineLabel();
            var missing = il.DefineLabel();
            Ldarg(il, instanceArg);
            il.Emit(OpCodes.Brfalse, missing);
            Ldarg(il, instanceArg);
            il.Emit(OpCodes.Castclass, property.DeclaringType);
            il.Emit(OpCodes.Callvirt, property.GetGetMethod());
            if (property.PropertyType.IsValueType) il.Emit(OpCodes.Box, property.PropertyType);
            il.Emit(OpCodes.Br, done);
            il.MarkLabel(missing);
            il.Emit(OpCodes.Ldnull);
            il.MarkLabel(done);
        }

        static void Ldarg(ILGenerator il, int index)
        {
            switch (index)
            {
                case 0: il.Emit(OpCodes.Ldarg_0); return;
                case 1: il.Emit(OpCodes.Ldarg_1); return;
                case 2: il.Emit(OpCodes.Ldarg_2); return;
                default: il.Emit(OpCodes.Ldarg_3); return;
            }
        }

        sealed class Draft
        {
            public TypeBuilder Type;

            public MethodBuilder Method;
        }

        static Draft Begin(Type type, string name, Type returnType, Type[] parameters)
        {
            var safe = (type.FullName ?? type.Name).Replace('.', '_').Replace('+', '_').Replace('`', '_').Replace('[', '_').Replace(']', '_').Replace(',', '_');
            var typeBuilder = Module.DefineType(name + "_" + safe + "_" + type.MetadataToken, TypeAttributes.Public | TypeAttributes.Class);
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

        static PropertyInfo[] Properties(Type type)
        {
            var list = new List<PropertyInfo>();
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.GetIndexParameters().Length > 0) throw new NotSupportedException("类型 " + type.FullName + " 含有索引器，改用反射。");
                if (!property.CanRead || !property.CanWrite) continue;
                if (property.GetGetMethod() == null || property.GetSetMethod() == null) throw new NotSupportedException("类型 " + type.FullName + " 含有非公共访问器，改用反射。");
                list.Add(property);
            }
            return list.ToArray();
        }
    }
}
