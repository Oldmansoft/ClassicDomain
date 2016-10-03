using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oldmansoft.ClassicDomain;

namespace UnitTest.ClassicDomain.Drvier.Mongo
{
    class Factory : IFactory
    {
        private UnitOfWork Uow { get; set; }

        public Factory()
        {
            Uow = new UnitOfWork();
        }

        public IUnitOfWork GetUnitOfWork()
        {
            return Uow;
        }

        public IRepositoryGet<Domain.Book, Guid> CreateBook()
        {
            return new Oldmansoft.ClassicDomain.Driver.Mongo.Repository<Domain.Book, Guid, Mapping>(Uow);
        }

        public IRepositoryGet<Domain.Book, Guid> CreateBook(string connectionName)
        {
            return new Oldmansoft.ClassicDomain.Driver.Mongo.Repository<Domain.Book, Guid, MappingCustomConnectionName, string>(Uow, connectionName);
        }
    }
}
