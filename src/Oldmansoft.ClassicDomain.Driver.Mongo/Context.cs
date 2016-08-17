using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.Mongo
{
    /// <summary>
    /// 实体上下文
    /// 数据库连接串格式 mongodb://[username:password@]host1[:port1][,host2[:port2],...[,hostN[:portN]]][/[database][?options]]
    /// 更多请参考 https://docs.mongodb.com/manual/reference/connection-string/
    /// </summary>
    public abstract class Context : FastModeContext
    {
        private static Config Server { get; set; }

        static Context()
        {
            Server = new Config();
        }

        /// <summary>
        /// 创建实体集
        /// </summary>
        /// <typeparam name="TDomain"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keyExpression"></param>
        /// <returns></returns>
        internal override FastModeDbSet<TDomain, TKey> CreateDbSet<TDomain, TKey>(System.Linq.Expressions.Expression<Func<TDomain, TKey>> keyExpression)
        {
            var database = Server.Get(ConnectionName).GetDatabase() as Library.MongoDatabase;
            var result = new SafeModeDbSet<TDomain, TKey>(database, keyExpression);
            result.IdentityMap.SetKey(keyExpression.Compile());
            database.SetIdentityMap(result.IdentityMap);
            return result;
        }
    }
}
