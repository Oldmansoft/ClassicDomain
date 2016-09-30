using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Library
{
    /// <summary>
    /// 更新项
    /// </summary>
    internal class UpdatedItem
    {
        /// <summary>
        /// 移除
        /// </summary>
        public List<string> Remove { get; private set; }

        /// <summary>
        /// 添加范围到列表
        /// </summary>
        public Dictionary<string, List<string>> AddRangeToList { get; private set; }

        /// <summary>
        /// 设置项在列表
        /// </summary>
        public Dictionary<string, Dictionary<int ,string>> SetItemInList { get; private set; }

        /// <summary>
        /// 移除列表的项
        /// </summary>
        public Dictionary<string, string> RemoveItemFromList { get; private set; }

        /// <summary>
        /// 设置范围在集合
        /// </summary>
        public Dictionary<string, string> SetRangeInHash { get; private set; }

        /// <summary>
        /// 设置范围在集合列表
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> SetRangeInHashes { get; private set; }

        /// <summary>
        /// 移除集合的实体
        /// </summary>
        public List<string> RemoveEntryFromHash { get; private set; }

        /// <summary>
        /// 移除集合列表的实体
        /// </summary>
        public Dictionary<string, List<string>> RemoveEntryFromHashes { get; private set; }

        public UpdatedItem()
        {
            Remove = new List<string>();
            AddRangeToList = new Dictionary<string, List<string>>();
            SetItemInList = new Dictionary<string, Dictionary<int, string>>();
            RemoveItemFromList = new Dictionary<string, string>();
            SetRangeInHash = new Dictionary<string, string>();
            SetRangeInHashes = new Dictionary<string, Dictionary<string, string>>();
            RemoveEntryFromHash = new List<string>();
            RemoveEntryFromHashes = new Dictionary<string, List<string>>();
        }
    }

    internal class UpdatedItem<TKey> : UpdatedItem
    {
        public TKey Key { get; private set; }

        public UpdatedItem(TKey key)
        {
            Key = key;
        }
    }
}
