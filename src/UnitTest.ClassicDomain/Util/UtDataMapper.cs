using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oldmansoft.ClassicDomain.Util;

namespace UnitTest.ClassicDomain.Util
{
    [TestClass]
    public class UtDataMapper
    {
        [TestMethod]
        public void TestIgnore()
        {
            var mapper = new DataMapper();
            mapper.SetIgnore<CopySourceModel>().Add(o => o.Name);

            var source = new CopySourceModel();
            source.SetName("hello");

            var target = mapper.CopyTo(source, new CopyTargetModel());
            Assert.IsNull(target.Name);
        }

        [TestMethod]
        public void TestDeepIgnore()
        {
            var mapper = new DataMapper();
            mapper.SetIgnore<CopySourceModel>().Add(o => o.Sub.Value);

            var source = new CopySourceModel();
            source.SetName("hello");
            source.CreateSub();
            source.Sub.Value = "world";

            var target = mapper.CopyTo(source, new CopySourceModel());
            Assert.IsNull(target.Sub.Value);
        }

        [TestMethod]
        public void TestPrivateContent()
        {
            var mapper = new DataMapper();
            var source = new CopySourceModel();
            source.SetName("hello");
            source.CreateSub();
            source.Sub.Value = "world";

            var target = mapper.CopyTo(source, new CopySourceModel());
            Assert.AreEqual("world", target.Sub.Value);
        }
    }
}
