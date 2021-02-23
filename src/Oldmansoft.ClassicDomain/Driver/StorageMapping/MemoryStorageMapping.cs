namespace Oldmansoft.ClassicDomain.Driver
{
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
}
