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

        /// <summary>
        /// 分页结果
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        public class PageResult<TData> : IPageResult<TData>
        {
            /// <summary>
            /// 分页数据
            /// </summary>
            public IList<TData> List { get; set; }

            /// <summary>
            /// 总数
            /// </summary>
            public int TotalCount { get; set; }
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
            var source = new ItemObject();
            source.Dic = new Dictionary<string, string>();
            source.Dic.Add("hello", "world");
            var target = source.CopyTo(new ItemObject());
            Assert.AreEqual(source.Dic.Count, target.Dic.Count);
        }

        class ItemObject
        {
            public Dictionary<string, string> Dic { get; set; }

            public string[] Array { get; set; }

            public List<string> List { get; set; }

            public CopySourceModel Model { get; set; }

            public E? Enum { get; set; }

            public enum E
            {
                A,
                B
            }
        }

        [TestMethod]
        public void TestCopyDicSourceNull()
        {
            var source = new ItemObject();
            var target = new ItemObject();
            target.Dic = new Dictionary<string, string>();
            source.CopyTo(target);
            Assert.IsNull(target.Dic);
        }

        [TestMethod]
        public void TestCopyArraySourceNull()
        {
            var source = new ItemObject();
            var target = new ItemObject();
            target.Array = new string[0];
            source.CopyTo(target);
            Assert.IsNull(target.Array);
        }

        [TestMethod]
        public void TestCopyListSourceNull()
        {
            var source = new ItemObject();
            var target = new ItemObject();
            target.List = new List<string>();
            source.CopyTo(target);
            Assert.IsNull(target.List);
        }

        [TestMethod]
        public void TestCopyModelSourceNull()
        {
            var source = new ItemObject();
            var target = new ItemObject();
            target.Model = new CopySourceModel();
            source.CopyTo(target);
            Assert.IsNull(target.Model);
        }

        [TestMethod]
        public void TestCopyEnumSourceNull()
        {
            var source = new ItemObject();
            var target = new ItemObject();
            target.Enum = ItemObject.E.B;
            source.CopyTo(target);
            Assert.IsNull(target.Enum);
        }

        [TestMethod]
        public void TestCopyIList()
        {
            IList<int> source = new List<int>();
            source.Add(1);
            var target = new List<int>();
            source.CopyTo(target);
            Assert.AreEqual(source.Count, target.Count);
        }
    }
}
