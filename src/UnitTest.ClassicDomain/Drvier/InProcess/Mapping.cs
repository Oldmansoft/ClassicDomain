using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.ClassicDomain.Drvier.InProcess
{
    class Mapping : Oldmansoft.ClassicDomain.Driver.InProcess.Context
    {
        protected override void OnModelCreating()
        {
            Add<Domain.Book, Guid>(o => o.Id);
        }
    }
}
