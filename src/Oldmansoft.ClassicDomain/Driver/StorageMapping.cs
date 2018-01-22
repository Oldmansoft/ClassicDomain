using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver
{
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
}
