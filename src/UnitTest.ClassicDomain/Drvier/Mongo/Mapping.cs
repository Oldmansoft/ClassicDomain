using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.ClassicDomain.Drvier.Mongo
{
    class Mapping : Oldmansoft.ClassicDomain.Driver.Mongo.Context
    {
        public override void OnModelCreating()
        {
            Add<Domain.Book, Guid>(o => o.Id);
        }
    }
}
