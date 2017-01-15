using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oldmansoft.ClassicDomain;
using UnitTest.ClassicDomain.Drvier.Domain;

namespace UnitTest.ClassicDomain.Drvier.Redis
{
    class FastModeFactory : IFactory
    {
        private UnitOfWork Uow { get; set; }

        public FastModeFactory()
        {
            Uow = new UnitOfWork();
        }

        public IUnitOfWork GetUnitOfWork()
        {
            return Uow;
        }

        public IRepository<Book, Guid> CreateBook()
        {
            return new Oldmansoft.ClassicDomain.Driver.Redis.Repository<Book, Guid, FastModeMapping>(Uow);
        }

        public IRepository<Book, Guid> CreateBook(string connectionName)
        {
            return new Oldmansoft.ClassicDomain.Driver.Redis.Repository<Book, Guid, FastModeMappingCustomConnectionName, string>(Uow, connectionName);
        }
    }
}
