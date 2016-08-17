using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oldmansoft.ClassicDomain.Driver.Mongo;

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
        public void TestHost()
        {
            var source = "mongodb://localhost/?replicaSet=test";
            var url = new MongoDB.Driver.MongoUrl(source);
            var setting = MongoDB.Driver.MongoServerSettings.FromUrl(url);
            Assert.AreEqual("localhost:27017", setting.GetHost());
        }
    }
}
