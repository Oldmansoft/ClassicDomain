using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oldmansoft.ClassicDomain;

namespace Sample.ConsoleApplication.Repositories
{
    class Mapping : Oldmansoft.ClassicDomain.Driver.Mongo.Context
    {
        protected override void OnModelCreating()
        {
            Add<Domain.Person, Guid>(o => o.Id)
                .SetUnique(g => g.CreateGroup(o => o.LastName).Add(o => o.FirstName));
        }
    }
}
