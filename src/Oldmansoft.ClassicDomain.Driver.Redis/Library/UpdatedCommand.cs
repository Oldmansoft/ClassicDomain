using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Library
{
    /// <summary>
    /// 更新命令集
    /// </summary>
    internal class UpdatedCommand
    {
        /// <summary>
        /// 移除
        /// </summary>
        public List<string> KeyDelete { get; private set; }

        /// <summary>
        /// 添加范围到列表
        /// </summary>
        public Dictionary<string, List<string>> ListRightPush { get; private set; }

        /// <summary>
        /// 设置项在列表
        /// </summary>
        public Dictionary<string, Dictionary<int, string>> ListSetByIndex { get; private set; }

        /// <summary>
        /// 移除列表的项
        /// </summary>
        public Dictionary<string, string> ListRemove { get; private set; }

        /// <summary>
        /// 设置范围在集合
        /// </summary>
        public Dictionary<string, string> HashSet { get; private set; }

        /// <summary>
        /// 设置范围在集合列表
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> HashSetList { get; private set; }

        /// <summary>
        /// 移除集合的实体
        /// </summary>
        public List<string> HashDelete { get; private set; }

        /// <summary>
        /// 移除集合列表的实体
        /// </summary>
        public Dictionary<string, List<string>> HashDeleteList { get; private set; }

        /// <summary>
        /// 创建更新命令集
        /// </summary>
        public UpdatedCommand()
        {
            KeyDelete = new List<string>();
            ListRightPush = new Dictionary<string, List<string>>();
            ListSetByIndex = new Dictionary<string, Dictionary<int, string>>();
            ListRemove = new Dictionary<string, string>();
            HashSet = new Dictionary<string, string>();
            HashSetList = new Dictionary<string, Dictionary<string, string>>();
            HashDelete = new List<string>();
            HashDeleteList = new Dictionary<string, List<string>>();
        }
    }

    /// <summary>
    /// 更新命令集
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    internal class UpdatedCommand<TKey> : UpdatedCommand
    {
        private Type DomainType { get; set; }

        /// <summary>
        /// 主键
        /// </summary>
        public TKey Key { get; private set; }

        /// <summary>
        /// 创建更新命令集
        /// </summary>
        /// <param name="key"></param>
        /// <param name="domainType"></param>
        public UpdatedCommand(TKey key, Type domainType)
        {
            Key = key;
            DomainType = domainType;
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="db"></param>
        /// <param name="dbSet"></param>
        public void Execute(IDatabase db, Core.IMergeKey<TKey> dbSet)
        {
            foreach (var item in KeyDelete)
            {
                db.KeyDelete(dbSet.MergeKey(Key, item));
            }
            foreach (var item in HashDelete)
            {
                db.HashDelete(dbSet.MergeKey(Key), item);
            }
            if (HashSet.Count > 0)
            {
                db.HashSet(dbSet.MergeKey(Key), HashSet.ToHashEntries());
            }
            foreach (var item in ListRightPush)
            {
                if (item.Value.Count == 0) continue;
                if (dbSet.IsLowServerVersion())
                {
                    foreach (var subItem in item.Value)
                    {
                        db.ListRightPush(dbSet.MergeKey(Key, item.Key), subItem);
                    }
                }
                else
                {
                    try
                    {
                        db.ListRightPush(dbSet.MergeKey(Key, item.Key), item.Value.ToRedisValues());
                    }
                    catch (RedisServerException ex)
                    {
                        if (ex.Message == "ERR wrong number of arguments for 'rpush' command")
                        {
                            throw new ClassicDomainException(DomainType, Core.ConfigStore.AlertLowServerVersion);
                        }
                        throw;
                    }
                }
            }
            foreach (var line in ListSetByIndex)
            {
                foreach (var item in line.Value)
                {
                    try
                    {
                        db.ListSetByIndex(dbSet.MergeKey(Key, line.Key), item.Key, item.Value);
                    }
                    catch (RedisServerException) { }
                }
            }
            foreach (var item in ListRemove)
            {
                db.ListRemove(dbSet.MergeKey(Key, item.Key), item.Value);
            }
            foreach (var item in HashSetList)
            {
                if (item.Value.Count == 0) continue;
                db.HashSet(dbSet.MergeKey(Key, item.Key), item.Value.ToHashEntries());
            }
            foreach (var item in HashDeleteList)
            {
                db.HashDelete(dbSet.MergeKey(Key, item.Key), item.Value.ToRedisValues());
            }
        }
    }
}
