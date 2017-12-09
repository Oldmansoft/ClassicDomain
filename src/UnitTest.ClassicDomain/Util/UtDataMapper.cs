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
            var source = new CopySourceModel();
            source.SetName("hello");
            source.CreateSub();
            source.Sub.Value = "world";

            var target = DataMapper.Map(source, new CopySourceModel());
            Assert.AreEqual("world", target.Sub.Value);
        }
    }
}
