using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oldmansoft.ClassicDomain;

namespace UnitTest.ClassicDomain.Drvier
{
    interface IFactory
    {
        IUnitOfWork GetUnitOfWork();

        IRepository<Domain.Book, Guid> CreateBook();

        IRepository<Domain.Book, Guid> CreateBook(string connectionName);
    }
}
