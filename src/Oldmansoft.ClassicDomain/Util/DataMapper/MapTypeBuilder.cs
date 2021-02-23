using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Oldmansoft.ClassicDomain.Util
{
    class MapTypeBuilder
    {
        private const string Namespace = "Oldmansoft.ClassicDomain.Util.DynamicMapper";

        private static readonly AssemblyBuilder AssemblyBuilder;

        private static readonly ModuleBuilder ModuleBuilder;

        static MapTypeBuilder()
        {
            AssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(Namespace), AssemblyBuilderAccess.Run);
            ModuleBuilder = AssemblyBuilder.DefineDynamicModule(Namespace);
        }

        public static Type CreateType(Type sourceType, Type targetType, PropertyInfo[] properties)
        {
            var typeBuilder = ModuleBuilder.DefineType(
                            string.Format("{0}._{1}_{2}", Namespace, sourceType.GetHashCode(), targetType.GetHashCode()),
                            TypeAttributes.Class | TypeAttributes.Public,
                            typeof(object),
                            new Type[] { typeof(IMap) }
                        );
            var genericParameters = typeBuilder.DefineGenericParameters("TSource", "TTarget");
            genericParameters[0].SetGenericParameterAttributes(GenericParameterAttributes.None);
            genericParameters[1].SetGenericParameterAttributes(GenericParameterAttributes.None);

            var fieldBuilders = new FieldBuilder[properties.Length];
            for (var i = 0; i < properties.Length; i++)
            {
                var propertyInfo = properties[i];
                var type = typeof(PropertyDelegateItem<,,>).MakeGenericType(genericParameters[0], genericParameters[1], propertyInfo.PropertyType);
                fieldBuilders[i] = typeBuilder.DefineField(propertyInfo.Name, type, FieldAttributes.Private);
            }

            DefineConstructor(typeBuilder, genericParameters[0], genericParameters[1], properties, fieldBuilders);
            var setContent = DefineSetContentMethod(typeBuilder, genericParameters[0], genericParameters[1], properties, fieldBuilders);
            DefineMapMethod(typeBuilder, setContent, genericParameters[0], genericParameters[1]);
            return typeBuilder.CreateTypeInfo().MakeGenericType(sourceType, targetType);
        }

        private static void DefineConstructor(TypeBuilder typeBuilder, GenericTypeParameterBuilder sourceType, GenericTypeParameterBuilder targetType, PropertyInfo[] properties, FieldBuilder[] fieldBuilders)
        {
            var objectConstructor = typeof(object).GetConstructor(Type.EmptyTypes);

            var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig, CallingConventions.Standard, Type.EmptyTypes);
            var gen = constructorBuilder.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Call, objectConstructor);

            for (var i = 0; i < properties.Length; i++)
            {
                var propertyInfo = properties[i];
                var type = typeof(PropertyDelegateItem<,,>);
                var constructor = type.GetConstructor(new Type[] { typeof(string) });
                var constructorHasType = TypeBuilder.GetConstructor(fieldBuilders[i].FieldType, constructor);

                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldstr, propertyInfo.Name);
                gen.Emit(OpCodes.Newobj, constructorHasType);
                gen.Emit(OpCodes.Stfld, fieldBuilders[i]);
            }

            gen.Emit(OpCodes.Ret);
        }

        private static MethodBuilder DefineSetContentMethod(TypeBuilder typeBuilder, GenericTypeParameterBuilder sourceType, GenericTypeParameterBuilder targetType, PropertyInfo[] properties, FieldBuilder[] fieldBuilders)
        {
            var methodBuilder = typeBuilder.DefineMethod(
                "SetContent",
                MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final,
                null,
                new Type[] { sourceType, targetType }
            );
            methodBuilder.DefineParameter(1, ParameterAttributes.None, "source");
            methodBuilder.DefineParameter(2, ParameterAttributes.None, "target");

            var gen = methodBuilder.GetILGenerator();
            for (var i = 0; i < fieldBuilders.Length; i++)
            {
                var type = typeof(PropertyDelegateItem<,,>);
                var setter = TypeBuilder.GetField(fieldBuilders[i].FieldType, type.GetField("Setter"));
                var getter = TypeBuilder.GetField(fieldBuilders[i].FieldType, type.GetField("Getter"));

                var getterInvoke = TypeBuilder.GetMethod(typeof(Func<,>).MakeGenericType(sourceType, properties[i].PropertyType), typeof(Func<,>).GetMethod("Invoke"));
                var setterInvoke = TypeBuilder.GetMethod(typeof(Action<,>).MakeGenericType(targetType, properties[i].PropertyType), typeof(Action<,>).GetMethod("Invoke"));

                var fieldBuilder = fieldBuilders[i];
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldfld, fieldBuilder);
                gen.Emit(OpCodes.Ldfld, setter);
                gen.Emit(OpCodes.Ldarg_2);
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldfld, fieldBuilder);
                gen.Emit(OpCodes.Ldfld, getter);
                gen.Emit(OpCodes.Ldarg_1);
                gen.Emit(OpCodes.Callvirt, getterInvoke);
                gen.Emit(OpCodes.Callvirt, setterInvoke);
            }
            gen.Emit(OpCodes.Ret);

            return methodBuilder;
        }

        private static void DefineMapMethod(TypeBuilder typeBuilder, MethodBuilder setContent, GenericTypeParameterBuilder sourceType, GenericTypeParameterBuilder targetType)
        {
            var methodBuilder = typeBuilder.DefineMethod(
                "Map",
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final,
                null,
                new Type[] { typeof(object), typeof(object) }
            );
            methodBuilder.DefineParameter(1, ParameterAttributes.None, "source");
            methodBuilder.DefineParameter(2, ParameterAttributes.None, "target");

            var gen = methodBuilder.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Unbox_Any, sourceType);
            gen.Emit(OpCodes.Ldarg_2);
            gen.Emit(OpCodes.Unbox_Any, targetType);
            gen.Emit(OpCodes.Call, setContent);
            gen.Emit(OpCodes.Ret);
        }
    }
}
