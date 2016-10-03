using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Core
{
    interface IMergeKey<TKey>
    {
        string MergeKey(TKey key);

        string MergeKey(TKey key, string subName);

        bool IsLowServerVersion();
    }
}
