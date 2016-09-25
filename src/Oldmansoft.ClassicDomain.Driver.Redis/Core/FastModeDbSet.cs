using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Core
{
    internal class FastModeDbSet<TDomain, TKey> : DbSet<TDomain, TKey>
    {
        /// <summary>
        /// 创建实体集
        /// </summary>
        /// <param name="config"></param>
        /// <param name="keyExpression"></param>
        public FastModeDbSet(ConfigItem config, Func<TDomain, TKey> keyExpression)
            : base(config, keyExpression)
        {
        }

        public override int Commit()
        {
            var result = 0;

            var db = Config.GetDatabase();
            TDomain domain;
            while (List.Addeds.TryDequeue(out domain))
            {
                if (db.StringSet(GetKey(domain), Serializer.Serialize(domain), null, When.NotExists)) result++;
            }
            while (List.Updateds.TryDequeue(out domain))
            {
                if (Config.IsLowServerVersion)
                {
                    if (db.KeyExists(GetKey(domain)) && db.StringSet(GetKey(domain), Serializer.Serialize(domain))) result++;
                }
                else
                {
                    try
                    {
                        if (db.StringSet(GetKey(domain), Serializer.Serialize(domain), null, When.Exists)) result++;
                    }
                    catch (RedisServerException ex)
                    {
                        if (ex.Message == "ERR wrong number of arguments for 'set' command")
                        {
                            throw new ClassicDomainException("接口不支持旧版服务器，请换用新的服务器，或在配置里指定 providerName=\"2\"。");
                        }
                    }
                }
            }
            while (List.Deleteds.TryDequeue(out domain))
            {
                if (db.KeyDelete(GetKey(domain))) result++;
            }

            return result;
        }

        public override TDomain Get(TKey id)
        {
            var content = Config.GetDatabase().StringGet(MergeKey(id));
            return Serializer.Deserialize<TDomain>(content);
        }
    }
}
