using System;
using System.Collections.Generic;
using System.Text;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Core
{
    interface IMergeKey<TKey>
    {
        string MergeKey(TKey key);

        string MergeKey(TKey key, string subName);

        bool IsLowServerVersion();
    }
}
