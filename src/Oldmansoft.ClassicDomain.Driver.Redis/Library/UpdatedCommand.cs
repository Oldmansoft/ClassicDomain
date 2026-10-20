using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        /// 主哈希里的二进制字段
        /// </summary>
        public Dictionary<string, byte[]> HashSetBinary { get; private set; }

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
            HashSetBinary = new Dictionary<string, byte[]>();
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
        /// 把本实体的写入放进同一次 pipeline
        /// </summary>
        /// <param name="target"></param>
        /// <param name="dbSet"></param>
        /// <param name="tasks"></param>
        public void Queue(IDatabaseAsync target, Core.IMergeKey<TKey> dbSet, ICollection<Task> tasks)
        {
            foreach (var item in KeyDelete)
            {
                tasks.Add(target.KeyDeleteAsync(dbSet.MergeKey(Key, item)));
            }
            if (HashDelete.Count > 0)
            {
                tasks.Add(target.HashDeleteAsync(dbSet.MergeKey(Key), HashDelete.Select(name => (RedisValue)name).ToArray()));
            }
            var entries = new List<HashEntry>();
            foreach (var item in HashSet)
            {
                entries.Add(new HashEntry(item.Key, item.Value));
            }
            foreach (var item in HashSetBinary)
            {
                entries.Add(new HashEntry(item.Key, item.Value));
            }
            if (entries.Count > 0)
            {
                tasks.Add(target.HashSetAsync(dbSet.MergeKey(Key), entries.ToArray()));
            }
            foreach (var item in ListRightPush)
            {
                if (item.Value.Count == 0) continue;
                var listKey = dbSet.MergeKey(Key, item.Key);
                if (dbSet.IsLowServerVersion())
                {
                    foreach (var subItem in item.Value)
                    {
                        tasks.Add(target.ListRightPushAsync(listKey, subItem));
                    }
                }
                else
                {
                    tasks.Add(target.ListRightPushAsync(listKey, item.Value.ToRedisValues()));
                }
            }
            foreach (var line in ListSetByIndex)
            {
                var listKey = dbSet.MergeKey(Key, line.Key);
                foreach (var item in line.Value)
                {
                    tasks.Add(target.ListSetByIndexAsync(listKey, item.Key, item.Value));
                }
            }
            foreach (var item in ListRemove)
            {
                tasks.Add(target.ListRemoveAsync(dbSet.MergeKey(Key, item.Key), item.Value));
            }
            foreach (var item in HashSetList)
            {
                if (item.Value.Count == 0) continue;
                tasks.Add(target.HashSetAsync(dbSet.MergeKey(Key, item.Key), item.Value.ToHashEntries()));
            }
            foreach (var item in HashDeleteList)
            {
                tasks.Add(target.HashDeleteAsync(dbSet.MergeKey(Key, item.Key), item.Value.ToRedisValues()));
            }
        }

        /// <summary>
        /// 等待 pipeline，并把旧版 Redis 的 rpush 错误翻译出来
        /// </summary>
        /// <param name="tasks"></param>
        public void Wait(ICollection<Task> tasks)
        {
            if (tasks.Count == 0) return;
            try
            {
                Task.WaitAll(tasks.ToArray());
            }
            catch (AggregateException ex)
            {
                foreach (var inner in ex.InnerExceptions)
                {
                    var redisError = inner as RedisServerException ?? inner.InnerException as RedisServerException;
                    if (redisError != null && redisError.Message == "ERR wrong number of arguments for 'rpush' command")
                    {
                        throw new ClassicDomainException(DomainType, Core.ConfigStore.AlertLowServerVersion);
                    }
                }
                throw;
            }
        }
    }
}
