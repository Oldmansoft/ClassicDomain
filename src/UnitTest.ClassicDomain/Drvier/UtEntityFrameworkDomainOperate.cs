using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.ClassicDomain.Drvier.EntityFramework
{
    [TestClass]
    public class UtEntityFrameworkDomainOperate
    {
        [TestMethod]
        public void TestAddReplaceRemove()
        {
            var factory = new Factory();
            var repository = factory.CreateBook();

            var domain = new Domain.Book();
            domain.Name = "hello";
            repository.Add(domain);
            factory.GetUnitOfWork().Commit();

            domain = repository.Get(domain.Id);
            Assert.IsNotNull(domain);

            domain.Name = "world";
            repository.Replace(domain);
            factory.GetUnitOfWork().Commit();

            domain = repository.Get(domain.Id);
            Assert.AreEqual("world", domain.Name);

            repository.Remove(domain);
            factory.GetUnitOfWork().Commit();

            domain = repository.Get(domain.Id);
            Assert.IsNull(domain);
        }

        [TestMethod]
        public void TestExecuteOrder()
        {
            var factory = new Factory();
            var repository = factory.CreateBookRepository();

            var domain = new Domain.Book();
            domain.Name = "hello";
            repository.Add(domain);

            repository.Execute((context) => {
                var find = context.Set<Domain.Book>().Find(domain.Id);
                Assert.AreEqual("hello", find.Name);
                return true;
            });

            domain.Name = "world";
            repository.Replace(domain);
            factory.GetUnitOfWork().Commit();

            domain = repository.Get(domain.Id);
            Assert.AreEqual("world", domain.Name);

            repository.Remove(domain);
            factory.GetUnitOfWork().Commit();
        }

        [TestMethod]
        public void TestPrimaryKey()
        {
            var factory = new Factory();
            var repository = factory.CreateBook();

            var list = new List<Guid>();
            for (var i = 0; i < 100; i++)
            {
                var domain = new Domain.Book();
                domain.Name = "hello";
                repository.Add(domain);
                list.Add(domain.Id);
            }
            factory.GetUnitOfWork().Commit();

            foreach (var item in list)
            {
                repository.Remove(repository.Get(item));
            }
            factory.GetUnitOfWork().Commit();
        }

        [TestMethod]
        public void TestNotEmptyPrimaryKey()
        {
            var factory = new Factory();
            var repository = factory.CreateBook();

            var id = Guid.NewGuid();
            var list = new List<Guid>();
            for (var i = 0; i < 100; i++)
            {
                var domain = new Domain.Book();
                if (i == 1) domain.Id = id;
                domain.Name = "hello";
                repository.Add(domain);
                list.Add(domain.Id);
            }
            factory.GetUnitOfWork().Commit();

            var data = repository.Get(id);
            foreach (var item in list)
            {
                repository.Remove(repository.Get(item));
            }
            factory.GetUnitOfWork().Commit();

            Assert.IsNotNull(data);
        }
    }
}
