using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oldmansoft.ClassicDomain;

namespace Sample.ConsoleApplication.Applications
{
    public class Persons
    {
        private Infrastructure.IRepositoryFactory Factory = Infrastructure.Container.Create<Infrastructure.IRepositoryFactory>();

        public bool Add(string firstName, string lastName, out Guid id)
        {
            var repository = Factory.CreatePerson();
            var domain = Domain.Person.Create(firstName, lastName);
            repository.Add(domain);
            try
            {
                Factory.GetUnitOfWork().Commit();
            }
            catch (UniqueException)
            {
                id = Guid.Empty;
                return false;
            }
            id = domain.Id;
            return true;
        }

        public void Change(Guid id, string firstName, string lastName)
        {
            var repository = Factory.CreatePerson();
            var domain = repository.Get(id);
            if (domain == null) return;
            domain.Change(firstName, lastName);
            repository.Replace(domain);
            try
            {
                Factory.GetUnitOfWork().Commit();
            }
            catch (UniqueException)
            {

            }
        }

        public void Remove(Guid id)
        {
            var repository = Factory.CreatePerson();
            var domain = repository.Get(id);
            repository.Remove(domain);
            Factory.GetUnitOfWork().Commit();
        }

        public IList<Data.PersonData> Page(int index, int size, out int totalCount)
        {
            var repository = Factory.CreatePerson();
            var result = repository.PageByName().Size(size).ToList(index, out totalCount);
            return result.MapTo(new List<Data.PersonData>());
        }
    }
}
