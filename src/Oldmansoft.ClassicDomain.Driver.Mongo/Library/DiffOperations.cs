using MongoDB.Bson;
using Oldmansoft.ClassicDomain.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Library
{
    static class DiffOperations
    {
        public static void DiffScalar<T>(T source, T target, string path, IUpdateBuffer buffer)
        {
            if (EqualityComparer<T>.Default.Equals(source, target)) return;
            if (!typeof(T).IsValueType && ReferenceEquals(target, null))
            {
                buffer.Unset(path);
                return;
            }
            buffer.Set(path, ToBson(target));
        }

        public static void DiffBytes(byte[] source, byte[] target, string path, IUpdateBuffer buffer)
        {
            if (source == null && target == null) return;
            if (target == null)
            {
                buffer.Unset(path);
                return;
            }
            if (source != null && source.SequenceEqual(target)) return;
            buffer.SetValue(path, target);
        }

        public static void DiffClass<T>(T source, T target, string path, IUpdateBuffer buffer) where T : class
        {
            if (source == null && target == null) return;
            if (target == null)
            {
                buffer.Unset(path);
                return;
            }
            if (source == null)
            {
                buffer.SetValue(path, target);
                return;
            }
            var diff = (Action<T, T, string, IUpdateBuffer>)DiffEmitter.Compile(typeof(T));
            diff(source, target, path, buffer);
        }

        public static void DiffList<T>(IList<T> source, IList<T> target, string path, IUpdateBuffer buffer)
        {
            if (source == null && target == null) return;
            if (target == null)
            {
                buffer.Unset(path);
                return;
            }
            if (source == null)
            {
                buffer.SetArray(path, ToBsonArray(target));
                return;
            }

            var shared = source.Count < target.Count ? source.Count : target.Count;
            for (var i = 0; i < shared; i++)
            {
                if (SameItem(source[i], target[i])) continue;
                buffer.SetArray(path, ToBsonArray(target));
                return;
            }
            if (target.Count == source.Count) return;
            if (target.Count > source.Count)
            {
                var extra = new List<T>(target.Count - source.Count);
                for (var i = source.Count; i < target.Count; i++) extra.Add(target[i]);
                buffer.PushEach(path, extra);
                return;
            }
            if (source.Count - target.Count == 1)
            {
                buffer.PopLast(path);
                return;
            }
            buffer.SetArray(path, ToBsonArray(target));
        }

        public static void DiffDictionary<TKey, TValue>(IDictionary<TKey, TValue> source, IDictionary<TKey, TValue> target, string path, IUpdateBuffer buffer)
        {
            if (source == null && target == null) return;
            if (target == null)
            {
                buffer.Unset(path);
                return;
            }
            if (source == null)
            {
                buffer.SetValue(path, target);
                return;
            }

            foreach (var pair in target)
            {
                var keyPath = ChildPath(path, pair.Key);
                if (!source.ContainsKey(pair.Key) || !SameItem(source[pair.Key], pair.Value))
                {
                    WriteValue(keyPath, pair.Value, buffer);
                }
            }
            foreach (var pair in source)
            {
                if (!target.ContainsKey(pair.Key)) buffer.Unset(ChildPath(path, pair.Key));
            }
        }

        public static bool SameClass<T>(T source, T target) where T : class
        {
            if (ReferenceEquals(source, target)) return true;
            if (source == null || target == null) return false;
            var same = (Func<T, T, bool>)DiffEmitter.CompileSame(typeof(T));
            return same(source, target);
        }

        public static bool SameList<T>(IList<T> source, IList<T> target)
        {
            if (source == null && target == null) return true;
            if (source == null || target == null || source.Count != target.Count) return false;
            for (var i = 0; i < source.Count; i++)
            {
                if (!SameItem(source[i], target[i])) return false;
            }
            return true;
        }

        public static bool SameDictionary<TKey, TValue>(IDictionary<TKey, TValue> source, IDictionary<TKey, TValue> target)
        {
            if (source == null && target == null) return true;
            if (source == null || target == null || source.Count != target.Count) return false;
            foreach (var pair in source)
            {
                if (!target.ContainsKey(pair.Key) || !SameItem(pair.Value, target[pair.Key])) return false;
            }
            return true;
        }

        public static bool SameScalar<T>(T source, T target)
        {
            return EqualityComparer<T>.Default.Equals(source, target);
        }

        public static bool SameBytes(byte[] source, byte[] target)
        {
            if (source == null && target == null) return true;
            if (source == null || target == null) return false;
            return source.SequenceEqual(target);
        }

        static bool SameItem<T>(T source, T target)
        {
            var type = typeof(T);
            if (type == typeof(byte[])) return SameBytes((byte[])(object)source, (byte[])(object)target);
            if (DiffEmitter.IsList(type)) return SameList((System.Collections.IList)(object)source, (System.Collections.IList)(object)target);
            if (DiffEmitter.IsDictionary(type)) return SameDictionary((System.Collections.IDictionary)(object)source, (System.Collections.IDictionary)(object)target);
            if (DiffEmitter.IsClass(type)) return SameClass((object)source, (object)target);
            return EqualityComparer<T>.Default.Equals(source, target);
        }

        static bool SameList(System.Collections.IList source, System.Collections.IList target)
        {
            if (source == null && target == null) return true;
            if (source == null || target == null || source.Count != target.Count) return false;
            for (var i = 0; i < source.Count; i++)
            {
                if (!Equals(source[i], target[i])) return false;
            }
            return true;
        }

        static bool SameDictionary(System.Collections.IDictionary source, System.Collections.IDictionary target)
        {
            if (source == null && target == null) return true;
            if (source == null || target == null || source.Count != target.Count) return false;
            foreach (System.Collections.DictionaryEntry pair in source)
            {
                if (!target.Contains(pair.Key) || !Equals(pair.Value, target[pair.Key])) return false;
            }
            return true;
        }

        static bool SameClass(object source, object target)
        {
            if (ReferenceEquals(source, target)) return true;
            if (source == null || target == null) return false;
            var same = DiffEmitter.CompileSame(source.GetType());
            return (bool)same.DynamicInvoke(source, target);
        }

        static void WriteValue<T>(string path, T value, IUpdateBuffer buffer)
        {
            if (!typeof(T).IsValueType && ReferenceEquals(value, null))
            {
                buffer.Unset(path);
                return;
            }
            if (DiffEmitter.IsClass(typeof(T)) || DiffEmitter.IsList(typeof(T)) || DiffEmitter.IsDictionary(typeof(T)))
            {
                buffer.SetValue(path, value);
                return;
            }
            buffer.Set(path, ToBson(value));
        }

        static string ChildPath(string path, object key)
        {
            var text = Convert.ToString(key, System.Globalization.CultureInfo.InvariantCulture);
            if (string.IsNullOrEmpty(text)) throw new ArgumentException(string.Format("属性 {0} 的键不允许为空", path));
            if (text.IndexOf('.') >= 0) throw new ArgumentException(string.Format("属性 {0} 的键不允许有字符“.”", path));
            if (text.IndexOf('$') >= 0) throw new ArgumentException(string.Format("属性 {0} 的键不允许有字符“$”", path));
            return path + "." + text;
        }

        static BsonArray ToBsonArray<T>(IList<T> values)
        {
            var array = new BsonArray();
            for (var i = 0; i < values.Count; i++) array.Add(ToBson(values[i]));
            return array;
        }

        static BsonValue ToBson<T>(T value)
        {
            if (ReferenceEquals(value, null)) return BsonNull.Value;
            if (DiffEmitter.IsClass(typeof(T))) return value.ToBsonDocument();
            return ((object)value).ToBsonValue();
        }
    }
}
