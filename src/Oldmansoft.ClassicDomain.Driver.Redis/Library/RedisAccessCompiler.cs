using System;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Library
{
    static class RedisAccessCompiler
    {
        static readonly ConcurrentDictionary<Type, Compiled> Cache = new ConcurrentDictionary<Type, Compiled>();

        public static bool TryDiff(Type type, object source, object target, UpdatedCommand result)
        {
            var compiled = For(type);
            if (compiled.Failed) return false;
            compiled.Diff(source, target, result, string.Empty);
            return true;
        }

        public static bool TryWrite(Type type, object source, UpdatedCommand result)
        {
            var compiled = For(type);
            if (compiled.Failed) return false;
            compiled.Write(source, result, string.Empty);
            return true;
        }

        public static bool TryRead(Type type, object instance, DataGetMapping mapping)
        {
            var compiled = For(type);
            if (compiled.Failed) return false;
            compiled.Read(instance, mapping, string.Empty);
            return true;
        }

        static Compiled For(Type type)
        {
            return Cache.GetOrAdd(type, Create);
        }

        static Compiled Create(Type type)
        {
            var compiled = (Compiled)Activator.CreateInstance(typeof(Compiled<>).MakeGenericType(type));
            return compiled;
        }

        abstract class Compiled
        {
            public bool Failed;

            public Action<object, object, UpdatedCommand, string> Diff;

            public Action<object, UpdatedCommand, string> Write;

            public Action<object, DataGetMapping, string> Read;
        }

        sealed class Compiled<T> : Compiled
        {
            public Compiled()
            {
                try
                {
                    Diff = (Action<object, object, UpdatedCommand, string>)RedisEmitter.CompileDiff(typeof(T));
                    Write = (Action<object, UpdatedCommand, string>)RedisEmitter.CompileWrite(typeof(T));
                    Read = (Action<object, DataGetMapping, string>)RedisEmitter.CompileRead(typeof(T));
                }
                catch (Exception ex)
                {
                    Failed = true;
                    Trace.TraceWarning("Redis 访问器生成失败，回退反射：{0}。{1}", typeof(T).FullName, ex.Message);
                }
            }
        }
    }
}
