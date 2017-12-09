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
            var list = Oldmansoft.ClassicDomain.ObjectCreator.CreateInstance<List<int>>();
            Assert.IsNotNull(list);
        }

        [TestMethod]
        public void TestIGenericList()
        {
            var list = Oldmansoft.ClassicDomain.ObjectCreator.CreateInstance<IList<int>>();
            Assert.IsNotNull(list);
        }

        [TestMethod]
        public void TestIGenericCollection()
        {
            var list = Oldmansoft.ClassicDomain.ObjectCreator.CreateInstance<ICollection<int>>();
            Assert.IsNotNull(list);
        }

        [TestMethod]
        public void TestGenericDictionary()
        {
            var list = Oldmansoft.ClassicDomain.ObjectCreator.CreateInstance<Dictionary<int, int>>();
            Assert.IsNotNull(list);
        }

        [TestMethod]
        public void TestIGenericDictionary()
        {
            var list = Oldmansoft.ClassicDomain.ObjectCreator.CreateInstance<IDictionary<int, int>>();
            Assert.IsNotNull(list);
        }

        [TestMethod]
        public void TestPublicCreateClass()
        {
            var obj = Oldmansoft.ClassicDomain.ObjectCreator.CreateInstance<PublicCreate>();
            Assert.IsNotNull(obj);
        }

        [TestMethod]
        public void TestPrivateCreateClass()
        {
            var obj = Oldmansoft.ClassicDomain.ObjectCreator.CreateInstance<PrivateCreate>();
            Assert.IsNotNull(obj);
        }

        [TestMethod]
        public void TestInternalPublicCreateClass()
        {
            var obj = Oldmansoft.ClassicDomain.ObjectCreator.CreateInstance<InternalPublicCreate>();
            Assert.IsNotNull(obj);
        }

        [TestMethod]
        public void TestInternalPrivateCreateClass()
        {
            var obj = Oldmansoft.ClassicDomain.ObjectCreator.CreateInstance<InternalPrivateCreate>();
            Assert.IsNotNull(obj);
        }
    }

    public class PublicCreate
    {
    }

    public class PrivateCreate
    {
        private PrivateCreate() { }
    }

    internal class InternalPublicCreate
    {
    }

    internal class InternalPrivateCreate
    {
        private InternalPrivateCreate() { }
    }
}
