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
        /// 更改列表
        /// </summary>
        private ChangeList<TDomain> ChangeList { get; set; }

        /// <summary>
        /// 创建实体集
        /// </summary>
        /// <param name="config"></param>
        /// <param name="db"></param>
        /// <param name="keyExpression"></param>
        public FastModeDbSet(ConfigItem config, IDatabase db, System.Linq.Expressions.Expression<Func<TDomain, TKey>> keyExpression)
            : base(config, db, keyExpression)
        {
            ChangeList = new ChangeList<TDomain>();
        }

        /// <summary>
        /// 注册添加
        /// </summary>
        /// <param name="domain"></param>
        public override void RegisterAdd(TDomain domain)
        {
            TrySetDomainKey(domain);
            ChangeList.Addeds.Enqueue(domain);
        }

        /// <summary>
        /// 注册替换
        /// </summary>
        /// <param name="domain"></param>
        public override void RegisterReplace(TDomain domain)
        {
            ChangeList.Updateds.Enqueue(domain);
        }

        /// <summary>
        /// 注册移除
        /// </summary>
        /// <param name="domain"></param>
        public override void RegisterRemove(TDomain domain)
        {
            ChangeList.Deleteds.Enqueue(domain);
        }

        public override int Commit()
        {
            var result = 0;

            TDomain domain;
            while (ChangeList.Addeds.TryDequeue(out domain))
            {
                if (Db.StringSet(GetKey(domain), Library.Serializer.Serialize(domain), null, When.NotExists)) result++;
            }
            while (ChangeList.Updateds.TryDequeue(out domain))
            {
                if (Config.IsLowServerVersion)
                {
                    if (Db.KeyExists(GetKey(domain)) && Db.StringSet(GetKey(domain), Library.Serializer.Serialize(domain))) result++;
                }
                else
                {
                    try
                    {
                        if (Db.StringSet(GetKey(domain), Library.Serializer.Serialize(domain), null, When.Exists)) result++;
                    }
                    catch (RedisServerException ex)
                    {
                        if (ex.Message == "ERR wrong number of arguments for 'set' command")
                        {
                            throw new ClassicDomainException(Core.Config.AlertLowServerVersion);
                        }
                        throw;
                    }
                }
            }
            while (ChangeList.Deleteds.TryDequeue(out domain))
            {
                if (Db.KeyDelete(GetKey(domain))) result++;
            }
            return result + base.Commit();
        }

        public override TDomain Get(TKey id)
        {
            var content = Db.StringGet(MergeKey(id));
            return Library.Serializer.Deserialize<TDomain>(content);
        }
    }
}
