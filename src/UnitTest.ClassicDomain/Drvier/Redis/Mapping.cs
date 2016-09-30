using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.ClassicDomain.Drvier.Redis
{
    class Mapping : Oldmansoft.ClassicDomain.Driver.Redis.Context
    {
        public override void OnModelCreating()
        {
            Add<Domain.Book, Guid>(o => o.Id);
        }
    }
}
