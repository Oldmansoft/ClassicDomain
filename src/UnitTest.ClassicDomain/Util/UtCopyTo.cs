using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oldmansoft.ClassicDomain;

namespace UnitTest.ClassicDomain.Util
{
    [TestClass]
    public class UtCopyTo
    {
        private CopySourceModel CreateSource(string name)
        {
            var source = new CopySourceModel();
            source.SetName(name);
            return source;
        }

        [TestMethod]
        public void TestCopyNormalClass()
        {
            var source = CreateSource("hello");
            var target = source.CopyTo(new CopyTargetModel());
            Assert.AreEqual(source.Name, target.Name);
        }
        
        [TestMethod]
        public void TestCopyArray()
        {
            var source = new CopySourceModel[1];
            source[0] = CreateSource("hello");

            var input = new CopyTargetModel[1];
            var target = source.CopyTo(input);

            Assert.AreEqual(input.GetHashCode(), target.GetHashCode());
            Assert.AreEqual(source.Length, target.Length);
            Assert.AreEqual("hello", target[0].Name);
        }

        [TestMethod]
        public void TestCopyIntArray()
        {
            var source = new int[1];
            source[0] = 1;

            var input = new int[1];
            var target = source.CopyTo(input);

            Assert.AreEqual(input.GetHashCode(), target.GetHashCode());
            Assert.AreEqual(source.Length, target.Length);
            Assert.AreEqual(1, target[0]);
        }

        [TestMethod]
        public void TestCopyIntStringArray()
        {
            var source = new int[1];
            source[0] = 1;

            var input = new string[1];
            var target = source.CopyTo(input);

            Assert.AreEqual(input.GetHashCode(), target.GetHashCode());
            Assert.AreEqual(source.Length, target.Length);
        }

        [TestMethod]
        public void TestCopyList()
        {
            var source = new List<CopySourceModel>();
            source.Add(CreateSource("hello"));

            var input = new List<CopyTargetModel>();
            var target = source.CopyTo(input);

            Assert.AreEqual(input.GetHashCode(), target.GetHashCode());
            Assert.AreEqual(source.Count, target.Count);
            Assert.AreEqual("hello", target[0].Name);
        }

        [TestMethod]
        public void TestCopyDictionary()
        {
            var source = new Dictionary<string, CopySourceModel>();
            source.Add("hello", CreateSource("world"));
            var input = new Dictionary<string, CopyTargetModel>();
            var target = source.CopyTo(input);

            Assert.AreEqual(input.GetHashCode(), target.GetHashCode());
            Assert.AreEqual(source.Count, target.Count);
            Assert.AreEqual("world", target["hello"].Name);
        }

        [TestMethod]
        public void TestCopyDiffKeyDictionary()
        {
            var source = new Dictionary<string, CopySourceModel>();
            source.Add("hello", CreateSource("world"));
            var input = new Dictionary<int, CopyTargetModel>();
            var target = source.CopyTo(input);

            Assert.AreEqual(input.GetHashCode(), target.GetHashCode());
            Assert.AreEqual(0, target.Count);
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

        [TestMethod]
        public void TestCopyItemList()
        {
            var source = new ItemObject();
            source.List = new List<string>();
            source.List.Add("hello");
            var target = source.CopyTo(new ItemObject());
            Assert.AreEqual(source.List.Count, target.List.Count);
        }

        [TestMethod]
        public void TestCopyItemArray()
        {
            var source = new ItemObject();
            source.Array = new string[1];
            source.Array[0] = "hello";
            var target = source.CopyTo(new ItemObject());
            Assert.AreEqual(source.Array.Length, target.Array.Length);
            Assert.AreEqual(source.Array[0], target.Array[0]);
        }

        class ItemObject
        {
            public Dictionary<string, string> Dic { get; set; }

            public string[] Array { get; set; }

            public List<string> List { get; set; }

            public CopySourceModel Model { get; set; }

            public E? NullEnum { get; set; }

            public E Enum { get; set; }

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
            target.NullEnum = ItemObject.E.B;
            source.CopyTo(target);
            Assert.IsNull(target.NullEnum);
        }

        [TestMethod]
        public void TestCopyEnum()
        {
            var source = new ItemObject();
            var target = new ItemObject();
            source.Enum = ItemObject.E.B;
            source.CopyTo(target);
            Assert.AreEqual(source.Enum, target.Enum);
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
