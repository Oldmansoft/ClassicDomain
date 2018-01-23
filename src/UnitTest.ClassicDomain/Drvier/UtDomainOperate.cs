﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.ClassicDomain.Drvier
{
    [TestClass]
    public class UtDomainOperate
    {
        [TestMethod]
        public void TestReplaceNull()
        {
            TestReplaceNull_Core(new Mongo.Factory());
            TestReplaceNull_Core(new Mongo.FastModeFactory());
            TestReplaceNull_Core(new Redis.Factory());
            TestReplaceNull_Core(new Redis.FastModeFactory());
        }

        private static void TestReplaceNull_Core(IFactory factory)
        {
            var repository = factory.CreateBook();

            var addDomain = new Domain.Book();
            addDomain.Name = "hello";
            addDomain.Authors = new List<Domain.Author>();
            addDomain.Authors.Add(new Domain.Author() { Name = "one" });
            addDomain.DictionaryValue = new Dictionary<string, string>();
            addDomain.DictionaryValue.Add("1", "1");
            repository.Add(addDomain);
            factory.GetUnitOfWork().Commit();

            var replaceDomain = repository.Get(addDomain.Id);
            replaceDomain.Name = null;
            replaceDomain.Authors = null;
            replaceDomain.DictionaryValue = null;
            repository.Replace(replaceDomain);
            factory.GetUnitOfWork().Commit();

            var getDomain = repository.Get(addDomain.Id);
            repository.Remove(getDomain);
            factory.GetUnitOfWork().Commit();

            var removeDomain = repository.Get(addDomain.Id);

            Assert.IsNotNull(replaceDomain);
            Assert.IsNull(getDomain.Authors);
            Assert.IsNull(removeDomain);
        }

        [TestMethod]
        public void TestReplaceNullToNew()
        {
            TestReplaceNullToNew_Core(new Mongo.Factory());
            TestReplaceNullToNew_Core(new Mongo.FastModeFactory());
            TestReplaceNullToNew_Core(new Redis.Factory());
            TestReplaceNullToNew_Core(new Redis.FastModeFactory());
        }

        private static void TestReplaceNullToNew_Core(IFactory factory)
        {
            var repository = factory.CreateBook();

            var addDomain = new Domain.Book();
            repository.Add(addDomain);
            factory.GetUnitOfWork().Commit();

            var replaceDomain = repository.Get(addDomain.Id);
            replaceDomain.Name = "hello";
            replaceDomain.Authors = new List<Domain.Author>();
            replaceDomain.Authors.Add(new Domain.Author() { Name = "one" });
            replaceDomain.DictionaryValue = new Dictionary<string, string>();
            replaceDomain.DictionaryValue.Add("1", "1");
            repository.Replace(replaceDomain);
            factory.GetUnitOfWork().Commit();

            var getDomain = repository.Get(addDomain.Id);
            repository.Remove(getDomain);
            factory.GetUnitOfWork().Commit();

            var removeDomain = repository.Get(addDomain.Id);

            Assert.IsNotNull(replaceDomain);
            Assert.IsNotNull(getDomain.Authors);
            Assert.IsNull(removeDomain);
        }

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

        [TestMethod]
        public void TestField()
        {
            TestField_Core(new Mongo.FastModeFactory());
        }

        private static void TestField_Core(IFactory factory)
        {
            var repository = factory.CreateBook();
            var addDomain = new Domain.Book();
            addDomain.FieldValue = "hello";
            repository.Add(addDomain);
            factory.GetUnitOfWork().Commit();
            
            var getDomain = repository.Get(addDomain.Id);
            repository.Remove(getDomain);
            factory.GetUnitOfWork().Commit();

            //Assert.AreEqual("hello", getDomain.FieldValue);
        }

        [TestMethod]
        public void TestReplaceBinaryByMongo()
        {
            var factory = new Mongo.Factory();
            var repository = factory.CreateBook();
            var domain = new Domain.Book();
            domain.Name = "hello";
            domain.Binary = new byte[] { 1 };
            repository.Add(domain);
            factory.GetUnitOfWork().Commit();
            domain = repository.Get(domain.Id);
            domain.Binary[0] += domain.Binary[0];
            repository.Replace(domain);
            factory.GetUnitOfWork().Commit();
            domain = repository.Get(domain.Id);
            repository.Remove(domain);
            factory.GetUnitOfWork().Commit();
            Assert.AreEqual(2, domain.Binary[0]);
        }

        [TestMethod]
        public void TestMongoExecuteOrder()
        {
            var factory = new Mongo.Factory();
            var repository = factory.CreateBookRepository();
            var domain = new Domain.Book();
            domain.Name = "hello";
            repository.Add(domain);
            repository.Execute((collection) => {
                collection.Update(MongoDB.Driver.Builders.Query.EQ("_id", domain.Id), MongoDB.Driver.Builders.Update.Set("FieldValue", "world"));
                return true;
            });
            domain.Name = "world";
            repository.Replace(domain);
            var result = factory.GetUnitOfWork().Commit();
            domain = repository.Get(domain.Id);
            repository.Remove(domain);
            factory.GetUnitOfWork().Commit();
            Assert.AreEqual(3, result);
            Assert.AreEqual("world", domain.FieldValue);
        }

        [TestMethod]
        public void TestExecuteOrder()
        {
            TestExecuteOrder_Core(new Mongo.Factory());
            TestExecuteOrder_Core(new Mongo.FastModeFactory());
        }

        private void TestExecuteOrder_Core(IFactory factory)
        {
            var domain = new Domain.Book();

            var repository = factory.CreateBook();
            domain.Name = "hello";
            repository.Add(domain);

            domain.Name = "world";
            repository.Replace(domain);

            var secondDomain = new Domain.Book();
            secondDomain.Name = "one";
            repository.Add(secondDomain);

            secondDomain.Name = "two";
            repository.Replace(secondDomain);

            factory.GetUnitOfWork().Commit();

            repository.Remove(domain);
            repository.Remove(secondDomain);
            factory.GetUnitOfWork().Commit();
        }
    }
}
