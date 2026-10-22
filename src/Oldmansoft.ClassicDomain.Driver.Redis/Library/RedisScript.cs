using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Library
{
    sealed class RedisPendingWrite
    {
        public RedisKey[] Keys;

        public RedisValue[] Arguments;

        public Action OnFailure;

        public Type DomainType;
    }

    static class RedisScript
    {
        public const string Source = @"
local mode = ARGV[1]
local main = KEYS[1]
if mode == 'exists' then
  if redis.call('HEXISTS', main, 'this') == 0 then
    return 0
  end
elseif mode == 'notexists' then
  if redis.call('HEXISTS', main, 'this') == 1 then
    return 0
  end
end
local i = 2
while i <= #ARGV do
  local op = ARGV[i]
  i = i + 1
  local key = KEYS[tonumber(ARGV[i])]
  i = i + 1
  if op == 'HSET' then
    local n = tonumber(ARGV[i])
    i = i + 1
    for _ = 1, n do
      redis.call('HSET', key, ARGV[i], ARGV[i + 1])
      i = i + 2
    end
  elseif op == 'HDEL' then
    local n = tonumber(ARGV[i])
    i = i + 1
    for _ = 1, n do
      redis.call('HDEL', key, ARGV[i])
      i = i + 1
    end
  elseif op == 'DEL' then
    redis.call('DEL', key)
  elseif op == 'RPUSH' then
    local n = tonumber(ARGV[i])
    i = i + 1
    for _ = 1, n do
      redis.call('RPUSH', key, ARGV[i])
      i = i + 1
    end
  elseif op == 'LSET' then
    local n = tonumber(ARGV[i])
    i = i + 1
    for _ = 1, n do
      redis.call('LSET', key, ARGV[i], ARGV[i + 1])
      i = i + 2
    end
  elseif op == 'LREM' then
    redis.call('LREM', key, 0, ARGV[i])
    i = i + 1
  else
    return redis.error_reply('unknown op')
  end
end
if mode == 'delete' then
  if redis.call('DEL', main) == 0 then
    return 0
  end
end
return 1
";

        public static RedisPendingWrite Encode<TKey>(string mode, TKey key, UpdatedCommand command, Core.IMergeKey<TKey> merge, string marker, Action onFailure, Type domainType)
        {
            var keys = new List<string>();
            var arguments = new List<RedisValue> { mode };
            var main = merge.MergeKey(key);
            Index(keys, main);

            var pairs = new List<RedisValue>();
            if (marker != null)
            {
                pairs.Add("this");
                pairs.Add(marker);
            }
            foreach (var item in command.HashSet)
            {
                pairs.Add(item.Key);
                pairs.Add(item.Value);
            }
            foreach (var item in command.HashSetBinary)
            {
                pairs.Add(item.Key);
                pairs.Add(item.Value);
            }
            if (pairs.Count > 0) Hset(arguments, keys, main, pairs);
            if (command.HashDelete.Count > 0)
            {
                arguments.Add("HDEL");
                arguments.Add(Index(keys, main));
                arguments.Add(command.HashDelete.Count);
                foreach (var name in command.HashDelete) arguments.Add(name);
            }
            foreach (var name in command.KeyDelete)
            {
                arguments.Add("DEL");
                arguments.Add(Index(keys, merge.MergeKey(key, name)));
            }
            foreach (var item in command.ListRightPush)
            {
                if (item.Value.Count == 0) continue;
                arguments.Add("RPUSH");
                arguments.Add(Index(keys, merge.MergeKey(key, item.Key)));
                arguments.Add(item.Value.Count);
                foreach (var value in item.Value) arguments.Add(value);
            }
            foreach (var line in command.ListSetByIndex)
            {
                if (line.Value.Count == 0) continue;
                arguments.Add("LSET");
                arguments.Add(Index(keys, merge.MergeKey(key, line.Key)));
                arguments.Add(line.Value.Count);
                foreach (var item in line.Value)
                {
                    arguments.Add(item.Key);
                    arguments.Add(item.Value);
                }
            }
            foreach (var item in command.ListRemove)
            {
                arguments.Add("LREM");
                arguments.Add(Index(keys, merge.MergeKey(key, item.Key)));
                arguments.Add(item.Value);
            }
            foreach (var item in command.HashSetList)
            {
                if (item.Value.Count == 0) continue;
                var sub = new List<RedisValue>();
                foreach (var pair in item.Value)
                {
                    sub.Add(pair.Key);
                    sub.Add(pair.Value);
                }
                Hset(arguments, keys, merge.MergeKey(key, item.Key), sub);
            }
            foreach (var item in command.HashDeleteList)
            {
                if (item.Value.Count == 0) continue;
                arguments.Add("HDEL");
                arguments.Add(Index(keys, merge.MergeKey(key, item.Key)));
                arguments.Add(item.Value.Count);
                foreach (var name in item.Value) arguments.Add(name);
            }

            var redisKeys = new RedisKey[keys.Count];
            for (var i = 0; i < keys.Count; i++) redisKeys[i] = keys[i];
            return new RedisPendingWrite
            {
                Keys = redisKeys,
                Arguments = arguments.ToArray(),
                OnFailure = onFailure,
                DomainType = domainType
            };
        }

        static void Hset(List<RedisValue> arguments, List<string> keys, string redisKey, List<RedisValue> pairs)
        {
            arguments.Add("HSET");
            arguments.Add(Index(keys, redisKey));
            arguments.Add(pairs.Count / 2);
            arguments.AddRange(pairs);
        }

        static int Index(List<string> keys, string redisKey)
        {
            var index = keys.IndexOf(redisKey);
            if (index >= 0) return index + 1;
            keys.Add(redisKey);
            return keys.Count;
        }
    }
}
