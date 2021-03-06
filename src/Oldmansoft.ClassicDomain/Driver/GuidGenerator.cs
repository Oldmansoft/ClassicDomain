﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Oldmansoft.ClassicDomain.Driver
{
    /// <summary>
    /// Guid 生成器
    /// </summary>
    public class GuidGenerator
    {
        /// <summary>
        /// 默认实例
        /// </summary>
        public static readonly GuidGenerator Default = new GuidGenerator();

        private byte[] Hash { get; set; }

        private uint Seed { get; set; }

        private byte[] LastTime { get; set; }

        private readonly object Locker = new object();

        /// <summary>
        /// 创建为内存排序用
        /// </summary>
        /// <returns></returns>
        public static Guid CreateForMemory()
        {
            return Default.Create(StorageMapping.MemoryMapping);
        }

        /// <summary>
        /// 创建为 MSSQL 数据库排序用
        /// </summary>
        /// <returns></returns>
        public static Guid CreateForMssql()
        {
            return Default.Create(StorageMapping.MssqlMapping);
        }

        /// <summary>
        /// 创建为 MongoDB 数据库排序用
        /// </summary>
        /// <returns></returns>
        public static Guid CreateForMongo()
        {
            return Default.Create(StorageMapping.MongoMapping);
        }

        /// <summary>
        /// 创建生成器
        /// </summary>
        public GuidGenerator()
        {
            var machineName = Environment.MachineName;
            var processId = System.Diagnostics.Process.GetCurrentProcess().Id;
            var domainId = AppDomain.CurrentDomain.Id;
            var buffer = new List<byte>();
            buffer.AddRange(Encoding.UTF8.GetBytes(machineName));
            buffer.AddRange(BitConverter.GetBytes(processId));
            buffer.AddRange(BitConverter.GetBytes(domainId));
            Hash = new System.Security.Cryptography.SHA256CryptoServiceProvider().ComputeHash(buffer.ToArray());
            Seed = 0;
            LastTime = new byte[6];
        }

        private uint GetNextSeed(byte[] time)
        {
            lock (Locker)
            {
                if (!IsSame(LastTime, time, 2))
                {
                    Seed = 0;
                    Array.Copy(time, 2, LastTime, 0, 6);
                }
                return Seed++;
            }
        }

        private bool IsSame(byte[] source, byte[] target, int targetOffset)
        {
            for (var i = 0; i < source.Length; i++)
            {
                if (source[i] != target[i + targetOffset]) return false;
            }
            return true;
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="mapping">存储映射器</param>
        /// <returns></returns>
        public Guid Create(IStorageMapping mapping)
        {
            var content = new byte[16];
            var time = BitConverter.GetBytes(DateTime.Now.Ticks);
            var seed = BitConverter.GetBytes(GetNextSeed(time));
            Array.Copy(Hash, 0, content, 0, 6);
            Array.Copy(seed, 0, content, 6, 4);
            Array.Copy(time, 2, content, 10, 6);

            if (mapping == null)
            {
                return new Guid(content);
            }
            else
            {
                return new Guid(StorageMapping.Format(content, mapping));
            }
        }
    }
}
