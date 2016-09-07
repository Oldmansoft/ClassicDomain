using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.ClassicDomain.Drvier.Mongo
{
    class Factory
    {
        private Oldmansoft.ClassicDomain.UnitOfWork Uow { get; set; }

        public Factory()
        {
            Uow = new Oldmansoft.ClassicDomain.UnitOfWork();
        }

        public Oldmansoft.ClassicDomain.IUnitOfWork GetUnitOfWork()
        {
            return Uow;
        }

        public Oldmansoft.ClassicDomain.IRepository<Domain.Book, Guid> CreateBook()
        {
            //TODO: 需要判断 Context 的类型是否符合
            return new Oldmansoft.ClassicDomain.Driver.Mongo.Repository<Domain.Book, Guid, Mapping>(Uow);
        }

        public Oldmansoft.ClassicDomain.IRepository<Domain.Book, Guid> CreateBook(string connectionName)
        {
            return new Oldmansoft.ClassicDomain.Driver.Mongo.Repository<Domain.Book, Guid, MappingCustomConnectionName, string>(Uow, connectionName);
        }
    }
}
