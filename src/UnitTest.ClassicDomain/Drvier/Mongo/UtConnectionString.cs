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
    }
}
