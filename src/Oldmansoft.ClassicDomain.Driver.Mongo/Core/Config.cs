using MongoDB.Driver;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Core
{
    /// <summary>
    /// 配置项
    /// </summary>
    internal class Config
    {
        /// <summary>
        /// 连接客户端
        /// </summary>
        private MongoClient Client { get; set; }

        /// <summary>
        /// 数据库名称
        /// </summary>
        private string DatabaseName { get; set; }

        /// <summary>
        /// 创建配置项
        /// </summary>
        /// <param name="client"></param>
        /// <param name="databaseName"></param>
        public Config(MongoClient client, string databaseName)
        {
            Client = client;
            DatabaseName = databaseName;
        }

        /// <summary>
        /// 获取数据库
        /// </summary>
        /// <returns></returns>
        public IMongoDatabase GetDatabase()
        {
            return Client.GetDatabase(DatabaseName);
        }
    }
}
