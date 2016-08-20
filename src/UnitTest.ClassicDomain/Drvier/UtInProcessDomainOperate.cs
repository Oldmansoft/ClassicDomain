using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.ClassicDomain.Drvier
{
    [TestClass]
    public class UtInProcessDomainOperate
    {
        [TestMethod]
        public void TestAdd()
        {
            var domain = new Domain.Book();
            domain.Id = Guid.NewGuid();
            domain.Name = "hello";
            var factory = new InProcess.Factory();
            factory.CreateBook().Add(domain);
            factory.GetUnitOfWork().Commit();

            var loadDomain = factory.CreateBook().Get(domain.Id);
            Assert.IsNotNull(loadDomain);
            Assert.AreEqual("hello", domain.Name);
        }

        [TestMethod]
        public void TestAddSameId()
        {
            var domain = new Domain.Book();
            domain.Id = Guid.NewGuid();
            domain.Name = "hello";
            var factory = new InProcess.Factory();
            factory.CreateBook().Add(domain);
            factory.GetUnitOfWork().Commit();

            factory.CreateBook().Add(domain);
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
