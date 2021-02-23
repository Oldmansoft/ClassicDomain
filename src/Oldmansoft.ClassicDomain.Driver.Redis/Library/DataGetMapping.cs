using System;
using System.Collections.Generic;
using System.Text;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Library
{
    class DataGetMapping
    {
        /// <summary>
        /// 普通字段
        /// </summary>
        public Dictionary<string, string> Fields { get; private set; }

        /// <summary>
        /// 列表字段
        /// </summary>
        public Dictionary<string, List<string>> Lists { get; private set; }

        /// <summary>
        /// 集合字段
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> Hashs { get; set; }

        public DataGetMapping(Dictionary<string, string> fields)
        {
            Fields = fields;
            Lists = new Dictionary<string, List<string>>();
            Hashs = new Dictionary<string, Dictionary<string, string>>();
        }
    }
}
