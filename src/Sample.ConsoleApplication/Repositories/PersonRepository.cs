using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oldmansoft.ClassicDomain;

namespace Sample.ConsoleApplication.Repositories
{
    class PersonRepository : Oldmansoft.ClassicDomain.Driver.Mongo.Repository<Domain.Person, Guid, Mapping>, Infrastructure.IPersonRepository
    {
        public PersonRepository(UnitOfWork uow)
            : base(uow)
        {
        }

        public IPagingData<Domain.Person> PageByName()
        {
            return Query().Paging().OrderBy(o => o.LastName).ThenBy(o => o.FirstName);
        }

        public Domain.Person GetByName(string firstName, string lastName)
        {
            return Query().FirstOrDefault(o => o.LastName == lastName && o.FirstName == firstName);
        }
    }
}
