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
            source.Name = "hello";

            var target = mapper.CopyTo(source, new CopyTargetModel());
            Assert.IsNull(target.Name);
        }
    }
}
