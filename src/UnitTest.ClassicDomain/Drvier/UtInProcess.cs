using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.ClassicDomain.Drvier
{
    [TestClass]
    public class UtInProcess
    {
        [TestMethod]
        public void TestAdd()
        {
            var domain = new InProcess.Domain();
            domain.Id = Guid.NewGuid();
            domain.Name = "hello";
            var factory = new InProcess.Factory();
            factory.CreateDomain().Add(domain);
            factory.GetUnitOfWork().Commit();

            var loadDomain = factory.CreateDomain().Get(domain.Id);
            Assert.IsNotNull(loadDomain);
            Assert.AreEqual("hello", domain.Name);
        }

        [TestMethod]
        public void TestAddSameId()
        {
            var domain = new InProcess.Domain();
            domain.Id = Guid.NewGuid();
            domain.Name = "hello";
            var factory = new InProcess.Factory();
            factory.CreateDomain().Add(domain);
            factory.GetUnitOfWork().Commit();

            factory.CreateDomain().Add(domain);
            try
            {
                factory.GetUnitOfWork().Commit();
            }
            catch (Oldmansoft.ClassicDomain.UniqueException)
            {
            }
        }
    }
}
