using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.ConsoleApplication.Infrastructure
{
    public interface IPersonRepository : Oldmansoft.ClassicDomain.IRepository<Domain.Person, Guid>
    {
        Oldmansoft.ClassicDomain.IPagingData<Domain.Person> PageByName();

        Domain.Person GetByName(string name);
    }
}
