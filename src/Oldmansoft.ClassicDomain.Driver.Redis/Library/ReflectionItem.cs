using System;
using System.Collections.Generic;
using System.Text;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Library
{
    class ReflectionItem
    {
        public List<string> ListNames { get; private set; }

        public List<string> HashNames { get; private set; }

        public ReflectionItem()
        {
            ListNames = new List<string>();
            HashNames = new List<string>();
        }
    }
}
