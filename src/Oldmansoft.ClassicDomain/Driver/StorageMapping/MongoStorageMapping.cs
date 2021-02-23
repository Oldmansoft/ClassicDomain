namespace Oldmansoft.ClassicDomain.Driver
{
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
