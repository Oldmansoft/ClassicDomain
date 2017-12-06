using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UnitTest.ClassicDomain.Util
{
    [TestClass]
    public class UtObjectCreator
    {
        [TestMethod]
        public void TestGenericList()
        {
            var list = Oldmansoft.ClassicDomain.Util.ObjectCreator.CreateInstance<List<int>>();
            Assert.IsNotNull(list);
        }

        [TestMethod]
        public void TestIGenericList()
        {
            var list = Oldmansoft.ClassicDomain.Util.ObjectCreator.CreateInstance<IList<int>>();
            Assert.IsNotNull(list);
        }

        [TestMethod]
        public void TestIGenericCollection()
        {
            var list = Oldmansoft.ClassicDomain.Util.ObjectCreator.CreateInstance<ICollection<int>>();
            Assert.IsNotNull(list);
        }

        [TestMethod]
        public void TestGenericDictionary()
        {
            var list = Oldmansoft.ClassicDomain.Util.ObjectCreator.CreateInstance<Dictionary<int, int>>();
            Assert.IsNotNull(list);
        }

        [TestMethod]
        public void TestIGenericDictionary()
        {
            var list = Oldmansoft.ClassicDomain.Util.ObjectCreator.CreateInstance<IDictionary<int, int>>();
            Assert.IsNotNull(list);
        }
    }
}
