using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver
{
    /// <summary>
    /// 更改列表
    /// </summary>
    /// <typeparam name="TDomain"></typeparam>
    public class ChangeList<TDomain>
    {
        /// <summary>
        /// 添加
        /// </summary>
        public ConcurrentQueue<TDomain> Addeds { get; private set; }

        /// <summary>
        /// 更新
        /// </summary>
        public ConcurrentQueue<TDomain> Updateds { get; private set; }

        /// <summary>
        /// 删除
        /// </summary>
        public ConcurrentQueue<TDomain> Deleteds { get; private set; }

        /// <summary>
        /// 刷新
        /// </summary>
        public ConcurrentQueue<TDomain> Refreshs { get; private set; }

        /// <summary>
        /// 创建更改列表
        /// </summary>
        public ChangeList()
        {
            Addeds = new ConcurrentQueue<TDomain>();
            Updateds = new ConcurrentQueue<TDomain>();
            Deleteds = new ConcurrentQueue<TDomain>();
            Refreshs = new ConcurrentQueue<TDomain>();
        }
    }
}
