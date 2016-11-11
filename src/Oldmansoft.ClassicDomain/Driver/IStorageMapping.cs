using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver
{
    /// <summary>
    /// 存储映射器
    /// </summary>
    public interface IStorageMapping
    {
        /// <summary>
        /// 获取映射
        /// </summary>
        /// <returns></returns>
        byte[] GetMapping();
    }

    /// <summary>
    /// 存储映射
    /// </summary>
    public class StorageMapping
    {
        /// <summary>
        /// 内存
        /// </summary>
        public static readonly IStorageMapping MemoryMapping = new MemoryStorageMapping();

        /// <summary>
        /// Mssql
        /// </summary>
        public static readonly IStorageMapping MssqlMapping = new MssqlStorageMapping();

        /// <summary>
        /// Mongo
        /// </summary>
        public static readonly IStorageMapping MongoMapping = new MongoStorageMapping();

        /// <summary>
        /// 格式化
        /// </summary>
        /// <param name="input"></param>
        /// <param name="mapping"></param>
        /// <returns></returns>
        public static byte[] Format(byte[] input, IStorageMapping mapping)
        {
            if (input == null) throw new ArgumentNullException("input");
            if (mapping == null) throw new ArgumentNullException("mapping");
            if (input.Length != 16) throw new ArgumentOutOfRangeException("input");

            var context = mapping.GetMapping();
            var result = new byte[16];
            for (var i = 0; i < result.Length; i++)
            {
                result[context[i]] = input[i];
            }
            return result;
        }
    }

    /// <summary>
    /// 内存存储映射器
    /// </summary>
    internal class MemoryStorageMapping : IStorageMapping
    {
        private static byte[] Mapping { get; set; }

        static MemoryStorageMapping()
        {
            Mapping = new byte[] { 15, 14, 13, 12, 11, 10, 9, 8, 6, 7, 4, 5, 0, 1, 2, 3 };
        }

        /// <summary>
        /// 获取映射器
        /// </summary>
        /// <returns></returns>
        public byte[] GetMapping()
        {
            return Mapping;
        }
    }

    /// <summary>
    /// MSSQL 存储映射器
    /// </summary>
    internal class MssqlStorageMapping : IStorageMapping
    {
        private static byte[] Mapping { get; set; }

        static MssqlStorageMapping()
        {
            Mapping = new byte[] { 3, 2, 1, 0, 5, 4, 7, 6, 9, 8, 15, 14, 13, 12, 11, 10 };
        }

        /// <summary>
        /// 获取映射器
        /// </summary>
        /// <returns></returns>
        public byte[] GetMapping()
        {
            return Mapping;
        }
    }

    /// <summary>
    /// Mongo 存储映射器
    /// </summary>
    internal class MongoStorageMapping : IStorageMapping
    {
        private static byte[] Mapping { get; set; }

        static MongoStorageMapping()
        {
            Mapping = new byte[] { 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };
        }

        /// <summary>
        /// 获取映射器
        /// </summary>
        /// <returns></returns>
        public byte[] GetMapping()
        {
            return Mapping;
        }
    }
}
