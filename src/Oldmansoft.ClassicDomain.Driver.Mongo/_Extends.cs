using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.Mongo
{
    static class Extends
    {
        public static string GetDatabase(this Uri source)
        {
            return source.AbsolutePath.Substring(1);
        }
    }
}
