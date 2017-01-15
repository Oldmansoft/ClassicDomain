using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oldmansoft.ClassicDomain;

namespace UnitTest.ClassicDomain.Drvier.Mongo
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
        
        public IRepository<Domain.Book, Guid> CreateBook()
        {
            return new Oldmansoft.ClassicDomain.Driver.Mongo.Repository<Domain.Book, Guid, FastModeMapping>(Uow);
        }

        public IRepository<Domain.Book, Guid> CreateBook(string connectionName)
        {
            return new Oldmansoft.ClassicDomain.Driver.Mongo.Repository<Domain.Book, Guid, FastModeMappingCustomConnectionName, string>(Uow, connectionName);
        }
    }
}
