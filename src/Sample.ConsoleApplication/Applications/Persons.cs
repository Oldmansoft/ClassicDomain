using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oldmansoft.ClassicDomain;
using Oldmansoft.ClassicDomain.Util;

namespace Sample.ConsoleApplication.Applications
{
    public class Persons
    {
        public Guid Add(Data.PersonData data)
        {
            var factory = new Repositories.RepositoryFactory();
            var repository = factory.CreatePerson();
            var domain = data.CopyTo(new Domain.Person());
            repository.Add(domain);
            factory.GetUnitOfWork().Commit();
            return domain.Id;
        }

        public void Edit(Data.PersonData data)
        {
            var factory = new Repositories.RepositoryFactory();
            var repository = factory.CreatePerson();
            var domain = repository.Query().FirstOrDefault(o => o.Id == data.Id);
            data.CopyTo(domain);
            repository.Replace(domain);
            factory.GetUnitOfWork().Commit();
        }

        public void Remove(Guid id)
        {
            var factory = new Repositories.RepositoryFactory();
            var repository = factory.CreatePerson();
            var domain = repository.Query().FirstOrDefault(o => o.Id == id);
            repository.Remove(domain);
            factory.GetUnitOfWork().Commit();
        }

        public Data.PersonData[] Page(int index, int size, out int totalCount)
        {
            var factory = new Repositories.RepositoryFactory();
            var repository = factory.CreatePerson();
            var result = repository.Paging().OrderBy(o => o.Name).Size(size).GetResult(out totalCount, index);
            return result.CopyTo(new Data.PersonData[result.Count]);
        }
    }
}
