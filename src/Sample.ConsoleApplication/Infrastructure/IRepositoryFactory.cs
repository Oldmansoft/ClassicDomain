using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oldmansoft.ClassicDomain;

namespace Sample.ConsoleApplication.Infrastructure
{
    interface IRepositoryFactory
    {
        IUnitOfWork GetUnitOfWork();

        IPersonRepository CreatePerson();
    }
}
