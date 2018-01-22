using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver
{
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
}
