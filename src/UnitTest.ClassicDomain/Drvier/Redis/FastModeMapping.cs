using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.ClassicDomain.Drvier.Redis
{
    class FastModeMapping : Oldmansoft.ClassicDomain.Driver.Redis.FastModeContext
    {
        protected override void OnModelCreating()
        {
            Add<Domain.Book, Guid>(o => o.Id);
        }
    }

    class FastModeMappingCustomConnectionName : Oldmansoft.ClassicDomain.Driver.Redis.FastModeContext<string>
    {
        protected override void OnModelCreating(string parameter)
        {
            ConnectionName = parameter;
            Add<Domain.Book, Guid>(o => o.Id);
        }
    }
}
