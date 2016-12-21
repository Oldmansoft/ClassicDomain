using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oldmansoft.ClassicDomain;

namespace Sample.ConsoleApplication.Repositories
{
    public class RepositoryFactory
    {
        private UnitOfWork Uow { get; set; }

        public RepositoryFactory()
        {
            Uow = new UnitOfWork();
        }

        public IUnitOfWork GetUnitOfWork()
        {
            return Uow;
        }

        public Infrastructure.IPersonRepository CreatePerson()
        {
            return new PersonRepository(Uow);
        }
    }
}
