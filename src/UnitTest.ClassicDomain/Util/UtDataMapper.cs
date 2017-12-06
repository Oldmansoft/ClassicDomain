using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oldmansoft.ClassicDomain.Util;

namespace UnitTest.ClassicDomain.Util
{
    [TestClass]
    public class UtDataMapper
    {
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
