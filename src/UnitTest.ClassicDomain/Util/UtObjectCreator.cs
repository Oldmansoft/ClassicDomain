using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UnitTest.ClassicDomain.Util
{
    [TestClass]
    public class UtObjectCreator
    {
        [TestMethod]
        public void TestIList()
        {
            var list = Oldmansoft.ClassicDomain.Util.ObjectCreator.CreateInstance<IList<int>>();
            Assert.IsNotNull(list);
        }

        [TestMethod]
        public void TestIDictionary()
        {
            var list = Oldmansoft.ClassicDomain.Util.ObjectCreator.CreateInstance<IDictionary<int, int>>();
            Assert.IsNotNull(list);
        }
    }
}
