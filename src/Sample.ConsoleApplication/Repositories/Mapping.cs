using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.ConsoleApplication.Repositories
{
    class Mapping : Oldmansoft.ClassicDomain.Driver.Mongo.Context
    {
        protected override void OnModelCreating()
        {
            Add<Domain.Person, Guid>(o => o.Id).SetUnique(o => o.Name);
        }
    }
}
