using MongoDB.Driver;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Core
{
    /// <summary>
    /// 配置项
    /// </summary>
    internal class Config
    {
        /// <summary>
        /// 服务
        /// </summary>
        private MongoServer Server { get; set; }

        /// <summary>
        /// 数据库名称
        /// </summary>
        private string DatabaseName { get; set; }

        /// <summary>
        /// 创建配置项
        /// </summary>
        /// <param name="server"></param>
        /// <param name="databaseName"></param>
        public Config(MongoServer server, string databaseName)
        {
            Server = server;
            DatabaseName = databaseName;
        }

        /// <summary>
        /// 获取数据库
        /// </summary>
        /// <returns></returns>
        public MongoDatabase GetDatabase()
        {
            return Server.GetDatabase(DatabaseName);
        }
    }
}
