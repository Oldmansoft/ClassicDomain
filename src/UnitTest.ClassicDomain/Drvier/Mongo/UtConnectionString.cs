using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oldmansoft.ClassicDomain;

namespace UnitTest.ClassicDomain.Drvier.Mongo
{
    [TestClass]
    public class UtConnectionString
    {
        [TestMethod]
        public void TestDatabaseName()
        {
            var source = "mongodb://localhost/database";
            Assert.AreEqual("database", new Uri(source).GetDatabase());
        }

        [TestMethod]
        public void TestConnectionString()
        {
            var connectionString = new Oldmansoft.ClassicDomain.Configuration.ConnectionString("test", @"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=UnitTest;Integrated Security=True", 0);
            Assert.AreEqual("UnitTest", connectionString.InitialCatalog);
        }
    }
}
