using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.ClassicDomain.Drvier.Mongo
{
    [TestClass]
    public class UtMongoDomainOperate
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
        public void TestListObjectRemoveFirst()
        {
            var factory = new Factory();
            var repository = factory.CreateBook();

            var domain = new Domain.Book();
            domain.Authors = new List<Domain.Author>();
            domain.Authors.Add(new Domain.Author() { Name = "one" });
            domain.Authors.Add(new Domain.Author() { Name = "two" });

            repository.Add(domain);
            factory.GetUnitOfWork().Commit();

            domain = repository.Get(domain.Id);

            domain.Authors.RemoveAt(0);
            repository.Replace(domain);
            factory.GetUnitOfWork().Commit();

            domain = repository.Get(domain.Id);
            Assert.AreEqual("two", domain.Authors[0].Name);
        }

        [TestMethod]
        public void TestListNormalRemoveFirst()
        {
            var factory = new Factory();
            var repository = factory.CreateBook();

            var domain = new Domain.Book();
            domain.Tags = new List<string>();
            domain.Tags.Add("hello");
            domain.Tags.Add("world");

            repository.Add(domain);
            factory.GetUnitOfWork().Commit();

            domain = repository.Get(domain.Id);

            domain.Tags.RemoveAt(0);
            repository.Replace(domain);
            factory.GetUnitOfWork().Commit();

            domain = repository.Get(domain.Id);
            Assert.AreEqual("world", domain.Tags[0]);
        }

        [TestMethod]
        public void TestListSet()
        {
            var factory = new Factory();
            var repository = factory.CreateBook();

            var domain = new Domain.Book();
            domain.Authors = new List<Domain.Author>();
            domain.Authors.Add(new Domain.Author() { Name = "one" });
            domain.Authors.Add(new Domain.Author() { Name = "two" });

            repository.Add(domain);
            factory.GetUnitOfWork().Commit();

            domain = repository.Get(domain.Id);

            domain.Authors[1].Name = "three";
            repository.Replace(domain);
            factory.GetUnitOfWork().Commit();

            domain = repository.Get(domain.Id);
            Assert.AreEqual("three", domain.Authors[1].Name);
        }
    }
}
