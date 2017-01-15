using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oldmansoft.ClassicDomain;
using UnitTest.ClassicDomain.Drvier.Domain;

namespace UnitTest.ClassicDomain.Drvier.EntityFramework
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

        public IRepository<Book, Guid> CreateBook()
        {
            return new Oldmansoft.ClassicDomain.Driver.EF.Repository<Book, Guid, Mapping>(Uow);
        }

        public IRepository<Book, Guid> CreateBook(string connectionName)
        {
            throw new NotImplementedException();
        }
    }
}
