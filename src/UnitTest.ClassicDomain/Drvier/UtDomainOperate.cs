using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.ClassicDomain.Drvier
{
    [TestClass]
    public class UtDomainOperate
    {
        [TestMethod]
        public void TestAddReplaceRemove()
        {
            TestAddReplaceRemove_Core(new Mongo.Factory());
            TestAddReplaceRemove_Core(new Mongo.FastModeFactory());
            TestAddReplaceRemove_Core(new Redis.Factory());
            TestAddReplaceRemove_Core(new Redis.FastModeFactory());
        }

        private static void TestAddReplaceRemove_Core(IFactory factory)
        {
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
        public void TestCustomConnectionName()
        {
            TestCustomConnectionName_Core(new Mongo.Factory(), "MongoCustomBook");
            TestCustomConnectionName_Core(new Mongo.FastModeFactory(), "MongoCustomBook");
            TestCustomConnectionName_Core(new Redis.Factory(), "RedisCustomBook");
            TestCustomConnectionName_Core(new Redis.FastModeFactory(), "RedisCustomBook");
        }

        private static void TestCustomConnectionName_Core(IFactory factory, string connectionName)
        {
            var repository = factory.CreateBook(connectionName);
            var domain = new Domain.Book();
            domain.Name = "hello";
            repository.Add(domain);
            factory.GetUnitOfWork().Commit();

            repository.Remove(domain);
            factory.GetUnitOfWork().Commit();

            domain = repository.Get(domain.Id);
            Assert.IsNull(domain);
        }

        [TestMethod]
        public void TestListObjectRemoveFirst()
        {
            TestListObjectRemoveFirst_Core(new Mongo.Factory());
            TestListObjectRemoveFirst_Core(new Mongo.FastModeFactory());
            TestListObjectRemoveFirst_Core(new Redis.Factory());
            TestListObjectRemoveFirst_Core(new Redis.FastModeFactory());
        }

        private static void TestListObjectRemoveFirst_Core(IFactory factory)
        {
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

            repository.Remove(domain);
            factory.GetUnitOfWork().Commit();

            domain = repository.Get(domain.Id);
            Assert.IsNull(domain);
        }

        [TestMethod]
        public void TestListNormalRemoveFirst()
        {
            TestListNormalRemoveFirst_Core(new Mongo.Factory());
            TestListNormalRemoveFirst_Core(new Mongo.FastModeFactory());
            TestListNormalRemoveFirst_Core(new Redis.Factory());
            TestListNormalRemoveFirst_Core(new Redis.FastModeFactory());
        }

        private static void TestListNormalRemoveFirst_Core(IFactory factory)
        {
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

            repository.Remove(domain);
            factory.GetUnitOfWork().Commit();

            domain = repository.Get(domain.Id);
            Assert.IsNull(domain);
        }

        [TestMethod]
        public void TestListSet()
        {
            TestListSet_Core(new Mongo.Factory());
            TestListSet_Core(new Mongo.FastModeFactory());
            TestListSet_Core(new Redis.Factory());
            TestListSet_Core(new Redis.FastModeFactory());
        }

        private static void TestListSet_Core(IFactory factory)
        {
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

            repository.Remove(domain);
            factory.GetUnitOfWork().Commit();

            domain = repository.Get(domain.Id);
            Assert.IsNull(domain);
        }

        [TestMethod]
        public void TestIdentityMap()
        {
            TestIdentityMap_Core(new Mongo.Factory());
            TestIdentityMap_Core(new Mongo.FastModeFactory());
            TestIdentityMap_Core(new Redis.Factory());
            TestIdentityMap_Core(new Redis.FastModeFactory());
        }

        private static void TestIdentityMap_Core(IFactory factory)
        {
            var repository = factory.CreateBook();

            var domain = new Domain.Book();
            domain.Sex = true;
            repository.Add(domain);
            factory.GetUnitOfWork().Commit();

            domain.Sex = false;
            repository.Replace(domain);
            factory.GetUnitOfWork().Commit();

            domain.Sex = true;
            repository.Replace(domain);
            factory.GetUnitOfWork().Commit();

            domain = repository.Get(domain.Id);
            
            repository.Remove(domain);
            factory.GetUnitOfWork().Commit();

            Assert.AreEqual(true, domain.Sex);
        }

        [TestMethod]
        public void TestRepository()
        {
            TestRepository_Core(new Mongo.Factory());
            TestRepository_Core(new Mongo.FastModeFactory());
            TestRepository_Core(new Redis.Factory());
            TestRepository_Core(new Redis.FastModeFactory());
        }

        private static void TestRepository_Core(IFactory factory)
        {
            var repository = factory.CreateBook();

            var domain = new Domain.Book();
            domain.Name = "hello";
            domain.Sex = true;
            repository.Add(domain);

            domain.Sex = false;
            repository.Replace(domain);

            domain.Sex = true;
            repository.Replace(domain);

            domain.Sex = false;
            repository.Replace(domain);
            var commit = factory.GetUnitOfWork().Commit();

            domain = repository.Get(domain.Id);

            repository.Remove(domain);
            factory.GetUnitOfWork().Commit();
            Assert.AreEqual(false, domain.Sex);
        }
    }
}
