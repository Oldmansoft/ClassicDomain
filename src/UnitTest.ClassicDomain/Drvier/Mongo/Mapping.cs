using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.ClassicDomain.Drvier.Mongo
{
    class Mapping : Oldmansoft.ClassicDomain.Driver.Mongo.Context
    {
        protected override void OnModelCreating()
        {
            Add<Domain.Book, Guid>(o => o.Id);
        }
    }

    class MappingCustomConnectionName : Oldmansoft.ClassicDomain.Driver.Mongo.Context<string>
    {
        protected override void OnModelCreating(string parameter)
        {
            ConnectionName = parameter;
            Add<Domain.Book, Guid>(o => o.Id);
        }
    }
}
