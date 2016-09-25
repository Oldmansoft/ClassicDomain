using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Core
{
    /// <summary>
    /// 配置
    /// </summary>
    internal class SafeModeConfig : Config
    {
        /// <summary>
        /// 创建服务器
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        protected override MongoServer CreateMongoServer(MongoServerSettings setting)
        {
            return new Library.MongoServer(setting);
        }
    }
}
