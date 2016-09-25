using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Core
{
    /// <summary>
    /// 快速模式配置
    /// </summary>
    internal class FastModeConfig : Config
    {
        /// <summary>
        /// 创建服务器
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        protected override MongoServer CreateMongoServer(MongoServerSettings setting)
        {
            return new MongoServer(setting);
        }
    }
}
