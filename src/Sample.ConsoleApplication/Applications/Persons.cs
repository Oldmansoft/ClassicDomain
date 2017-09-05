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
        public bool Add(string name, out Guid id)
        {
            var factory = new Repositories.RepositoryFactory();
            var repository = factory.CreatePerson();
            var domain = Domain.Person.Create(name);
            repository.Add(domain);
            try
            {
                factory.GetUnitOfWork().Commit();
            }
            catch (UniqueException)
            {
                id = Guid.Empty;
                return false;
            }
            id = domain.Id;
            return true;
        }

        public void Edit(Guid id, string name)
        {
            var factory = new Repositories.RepositoryFactory();
            var repository = factory.CreatePerson();
            var domain = repository.Get(id);
            if (domain == null) return;
            domain.Change(name);
            repository.Replace(domain);
            try
            {
                factory.GetUnitOfWork().Commit();
            }
            catch (UniqueException)
            {

            }
        }

        public void Remove(Guid id)
        {
            var factory = new Repositories.RepositoryFactory();
            var repository = factory.CreatePerson();
            var domain = repository.Get(id);
            repository.Remove(domain);
            factory.GetUnitOfWork().Commit();
        }

        public IList<Data.PersonData> Page(int index, int size, out int totalCount)
        {
            var factory = new Repositories.RepositoryFactory();
            var repository = factory.CreatePerson();
            var result = repository.PageByName().Size(size).ToList(index, out totalCount);
            return result.CopyTo(new List<Data.PersonData>());
        }
    }
}
