using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oldmansoft.ClassicDomain;
using Oldmansoft.ClassicDomain.Util;

namespace UnitTest.ClassicDomain.Util
{
    [TestClass]
    public class UtCopyTo
    {
        [TestMethod]
        public void TestCopyNormalClass()
        {
            var source = new CopySourceModel();
            source.Name = "hello";
            var target = source.CopyTo(new CopyTargetModel());
            Assert.AreEqual(source.Name, target.Name);
        }

        [TestMethod]
        public void TestCopyPageResult()
        {
            var source = new PageResult<CopySourceModel>();
            var list =  new List<CopySourceModel>();
            list.Add(new CopySourceModel() { Name = "hello" });
            source.List = list;
            source.TotalCount = 1;

            var target = source.CopyTo(new PageResult<CopyTargetModel>());

            Assert.AreEqual(source.TotalCount, target.TotalCount);
            Assert.AreEqual(source.List.Count(), target.List.Count());
        }

        [TestMethod]
        public void TestCopyZeroPageResult()
        {
            var source = new PageResult<CopySourceModel>();
            source.List = new List<CopySourceModel>();
            source.TotalCount = 0;

            var target = source.CopyTo(new PageResult<CopyTargetModel>());

            Assert.AreEqual(source.TotalCount, target.TotalCount);
            Assert.AreEqual(source.List.Count(), target.List.Count());
        }
    }
}
