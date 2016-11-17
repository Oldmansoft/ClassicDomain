using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oldmansoft.ClassicDomain;
using Oldmansoft.ClassicDomain.Driver;

namespace UnitTest.ClassicDomain.Drvier
{
    [TestClass]
    public class UtGuidGenerator
    {
        [TestMethod]
        public void TestMemoryGuidSorted()
        {
            var sorted = new List<GuidData>();
            for (var i = 0; i < 16; i++)
            {
                var input = new byte[16];
                input[i] = 1;
                sorted.Add(new GuidData { Id = new Guid(StorageMapping.Format(input, StorageMapping.MemoryMapping)), Index = i });
            }
            var output = sorted.OrderBy(o => o.Id).ToList();
            for (var i = 0; i < output.Count; i++)
            {
                Assert.AreEqual(i, output[i].Index);
            }
        }

        [TestMethod]
        public void TestMssqlGuidSorted()
        {
            var factory = new Factory();
            var repository = factory.CreateEf();
            for (var i = 0; i < 16; i++)
            {
                var input = new byte[16];
                input[i] = 1;
                repository.Add(new GuidData { Id = new Guid(StorageMapping.Format(input, StorageMapping.MssqlMapping)), Index = i });
            }
            factory.GetUnitOfWork().Commit();
            var output = repository.Query().OrderBy(o => o.Id).ToList();
            foreach(var item in output)
            {
                repository.Remove(item);
            }
            factory.GetUnitOfWork().Commit();
            for (var i = 0; i < output.Count; i++)
            {
                Assert.AreEqual(i, output[i].Index);
            }
        }

        [TestMethod]
        public void TestMongoGuidSorted()
        {
            var factory = new Factory();
            var repository = factory.CreateMongo();
            for (var i = 0; i < 16; i++)
            {
                var input = new byte[16];
                input[i] = 1;
                repository.Add(new GuidData { Id = new Guid(StorageMapping.Format(input, StorageMapping.MongoMapping)), Index = i });
            }
            factory.GetUnitOfWork().Commit();
            var output = repository.Query().OrderBy(o => o.Id).ToList();
            foreach (var item in output)
            {
                repository.Remove(item);
            }
            factory.GetUnitOfWork().Commit();
            for (var i = 0; i < output.Count; i++)
            {
                Assert.AreEqual(i, output[i].Index);
            }
        }

        [TestMethod]
        public void TestMemoryGuidGenerator()
        {
            var sorted = new List<GuidData>();
            for (var i = 0; i < 10000; i++)
            {
                sorted.Add(new GuidData { Id = GuidGenerator.Default.Create(StorageMapping.MemoryMapping), Index = i });
            }
            var output = sorted.OrderBy(o => o.Id).ToList();
            for (var i = 0; i < output.Count; i++)
            {
                Assert.AreEqual(i, output[i].Index);
            }
        }

        [TestMethod]
        public void TestMssqlGuidGenerator()
        {
            var factory = new Factory();
            var repository = factory.CreateEf();
            for (var i = 0; i < 1000; i++)
            {
                repository.Add(new GuidData { Id = GuidGenerator.Default.Create(StorageMapping.MssqlMapping), Index = i });
            }
            factory.GetUnitOfWork().Commit();
            var output = repository.Query().OrderBy(o => o.Id).ToList();
            foreach (var item in output)
            {
                repository.Remove(item);
            }
            factory.GetUnitOfWork().Commit();
            for (var i = 0; i < output.Count; i++)
            {
                Assert.AreEqual(i, output[i].Index);
            }
        }

        [TestMethod]
        public void TestMongoGuidGenerator()
        {
            var factory = new Factory();
            var repository = factory.CreateMongo();
            for (var i = 0; i < 10000; i++)
            {
                repository.Add(new GuidData { Id = GuidGenerator.Default.Create(StorageMapping.MongoMapping), Index = i });
            }
            factory.GetUnitOfWork().Commit();
            var output = repository.Query().OrderBy(o => o.Id).ToList();
            foreach (var item in output)
            {
                repository.Remove(item);
            }
            factory.GetUnitOfWork().Commit();
            for (var i = 0; i < output.Count; i++)
            {
                Assert.AreEqual(i, output[i].Index);
            }
        }

        class Factory
        {
            private UnitOfWork Uow { get; set; }

            public Factory()
            {
                Uow = new UnitOfWork();
            }

            public IUnitOfWork GetUnitOfWork()
            {
                return Uow;
            }

            public IRepository<GuidData, Guid> CreateEf()
            {
                return new Oldmansoft.ClassicDomain.Driver.EF.Repository<GuidData, Guid, EfMapping>(Uow);
            }

            public IRepository<GuidData, Guid> CreateMongo()
            {
                return new Oldmansoft.ClassicDomain.Driver.Mongo.Repository<GuidData, Guid, MongoMapping>(Uow);
            }
        }
    }

    class GuidData
    {
        public Guid Id { get; set; }

        public int Index { get; set; }
    }

    class EfMapping : Oldmansoft.ClassicDomain.Driver.EF.Context
    {
        protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.PluralizingTableNameConvention>();

            modelBuilder.Entity<GuidData>().Property(o => o.Id).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
        }
    }

    class MongoMapping : Oldmansoft.ClassicDomain.Driver.Mongo.Context
    {
        protected override void OnModelCreating()
        {
            Add<GuidData, Guid>(o => o.Id);
        }
    }
}
