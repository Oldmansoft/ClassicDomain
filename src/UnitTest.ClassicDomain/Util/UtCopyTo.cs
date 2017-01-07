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

        [TestMethod]
        public void TestCopyArray()
        {
            var source = new CopySourceModel[1];
            source[0] = new CopySourceModel() { Name = "hello" };

            var target = source.CopyTo(new CopyTargetModel[1]);

            Assert.AreEqual(source.Length, target.Length);
            Assert.AreEqual("hello", target[0].Name);
        }

        [TestMethod]
        public void TestCopyList()
        {
            var source = new List<CopySourceModel>();
            source.Add(new CopySourceModel() { Name = "hello" });

            var target = source.CopyTo(new List<CopyTargetModel>());

            Assert.AreEqual(source.Count, target.Count);
            Assert.AreEqual("hello", target[0].Name);
        }

        [TestMethod]
        public void TestCopyDictionary()
        {
            var source = new Dictionary<string, CopySourceModel>();
            source.Add("hello", new CopySourceModel() { Name = "world" });
            var target = source.CopyTo(new Dictionary<string, CopyTargetModel>());

            Assert.AreEqual(source.Count, target.Count);
            Assert.AreEqual("world", target["hello"].Name);
        }

        [TestMethod]
        public void TestCopyItemDictionary()
        {
            var source = new ItemDictionary();
            source.Item = new Dictionary<string, string>();
            source.Item.Add("hello", "world");
            var target = source.CopyTo(new ItemDictionary());
            Assert.AreEqual(source.Item.Count, target.Item.Count);
        }

        class ItemDictionary
        {
            public Dictionary<string, string> Item { get; set; }
        }
    }
}
