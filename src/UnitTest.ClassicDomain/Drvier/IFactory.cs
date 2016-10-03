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

        IRepositoryGet<Domain.Book, Guid> CreateBook();

        IRepositoryGet<Domain.Book, Guid> CreateBook(string connectionName);
    }
}
