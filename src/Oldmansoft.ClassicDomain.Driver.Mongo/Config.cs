using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Oldmansoft.ClassicDomain.Driver.Mongo
{
    /// <summary>
    /// 配置
    /// </summary>
    class Config : FastModeConfig
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
