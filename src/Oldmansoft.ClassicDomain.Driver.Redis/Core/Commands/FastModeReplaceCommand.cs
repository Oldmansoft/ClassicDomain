using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Core.Commands
{
    class FastModeReplaceCommand<TDomain> : ICommand
    {
        private IDatabase Db;

        private Func<TDomain, string> GetKey;

        private ConfigItem Config;

        private TDomain Domain;

        public FastModeReplaceCommand(IDatabase db, Func<TDomain, string> getKey, ConfigItem config, TDomain domain)
        {
            Db = db;
            GetKey = getKey;
            Config = config;
            Domain = domain;
        }

        public Type Type
        {
            get
            {
                return typeof(TDomain);
            }
        }

        public bool Execute()
        {
            var key = GetKey(Domain);
            if (Config.IsLowServerVersion)
            {
                return Db.KeyExists(key) && Db.StringSet(key, Library.Serializer.Serialize(Domain));
            }
            else
            {
                try
                {
                    return Db.StringSet(key, Library.Serializer.Serialize(Domain), null, When.Exists);
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
    }
}
