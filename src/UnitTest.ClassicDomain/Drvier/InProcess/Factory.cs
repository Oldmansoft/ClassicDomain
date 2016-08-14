using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.ClassicDomain.Drvier.InProcess
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

        public Oldmansoft.ClassicDomain.IRepositoryGet<Domain, Guid> CreateDomain()
        {
            return new Oldmansoft.ClassicDomain.Driver.InProcess.Repository<Domain, Guid>(Uow.Get<Mapping>());
        }
    }
}
