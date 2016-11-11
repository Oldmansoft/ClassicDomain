using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver
{
    /// <summary>
    /// Guid 生成器
    /// </summary>
    public class GuidGenerator
    {
        private byte[] Hash { get; set; }

        private short Seed { get; set; }

        private byte[] LastTime { get; set; }

        /// <summary>
        /// 默认实例
        /// </summary>
        public static readonly GuidGenerator Default = new GuidGenerator();

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

        private short GetNextSeed(byte[] time)
        {
            lock (this)
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
            Array.Copy(Hash, 0, content, 0, 8);
            Array.Copy(seed, 0, content, 8, 2);
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
