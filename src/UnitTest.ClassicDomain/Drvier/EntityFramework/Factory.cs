using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.ClassicDomain.Drvier.EntityFramework
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

        public Oldmansoft.ClassicDomain.IRepositoryGet<Domain.Book, Guid> CreateBook()
        {
            return new Oldmansoft.ClassicDomain.Driver.EF.Repository<Domain.Book, Guid, Mapping>(Uow);
        }
    }
}
