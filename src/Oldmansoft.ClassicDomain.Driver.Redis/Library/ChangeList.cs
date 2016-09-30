using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Library
{
    /// <summary>
    /// 更新列表
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    internal class ChangeList<TKey>
    {
        /// <summary>
        /// 添加列表
        /// </summary>
        public ConcurrentQueue<UpdatedItem<TKey>> Addes { get; private set; }

        /// <summary>
        /// 替换列表
        /// </summary>
        public ConcurrentQueue<UpdatedItem<TKey>> Replaces { get; private set; }

        /// <summary>
        /// 移除列表
        /// </summary>
        public ConcurrentQueue<UpdatedItem<TKey>> Removes { get; private set; }

        /// <summary>
        /// 创建更新列表
        /// </summary>
        public ChangeList()
        {
            Addes = new ConcurrentQueue<UpdatedItem<TKey>>();
            Replaces = new ConcurrentQueue<UpdatedItem<TKey>>();
            Removes = new ConcurrentQueue<UpdatedItem<TKey>>();
        }
    }
}
