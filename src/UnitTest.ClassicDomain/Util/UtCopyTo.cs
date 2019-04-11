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
        [TestMethod]
        public void TestPrivateContent()
        {
            var source = new CopySourceModel();
            source.SetName("hello");
            source.CreateSub();
            source.Sub.Value = "world";

            var target = source.MapTo(new CopySourceModel());
            Assert.AreEqual("world", target.Sub.Value);
        }

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
            var target = source.MapTo(new CopyTargetModel());
            Assert.AreEqual(source.Name, target.Name);
        }

        [TestMethod]
        public void TestCopySubClass()
        {
            var source = CreateSource("hello");
            source.CreateSub();
            source.Sub.Value = "world";
            var target = source.MapTo(new CopySourceModel());
            Assert.AreNotEqual(source.Sub, target.Sub);
            Assert.AreEqual(source.Sub.Value, target.Sub.Value);
        }

        [TestMethod]
        public void TestCopyTargetNull()
        {
            var source = new CopySourceModel[1];
            source[0] = CreateSource("hello");
            CopyTargetModel[] target = null;
            target = source.MapTo(target);
            Assert.IsNull(target);
        }

        [TestMethod]
        public void TestCopyArray()
        {
            var source = new CopySourceModel[2];
            source[0] = CreateSource("hello");
            source[1] = CreateSource("world");

            var input = new CopyTargetModel[2];
            var target = source.MapTo(input);

            Assert.AreEqual(input.GetHashCode(), target.GetHashCode());
            for (var i = 0; i < source.Length; i++)
            {
                Assert.AreEqual(source[i].Name, target[i].Name);
            }
        }

        [TestMethod]
        public void TestCopyPropertyArray()
        {
            var source = new CopySourceModel();
            source.SubArray = new CopySourceSubModel[2];
            source.SubArray[0] = CopySourceSubModel.Create("hello");
            source.SubArray[1] = CopySourceSubModel.Create("world");

            var input = new CopyTargetModel();
            var target = source.MapTo(input);

            Assert.AreEqual(input.GetHashCode(), target.GetHashCode());
            for (var i = 0; i < source.SubArray.Length; i++)
            {
                Assert.AreEqual(source.SubArray[i].Value, target.SubArray[i].Value);
            }
        }

        [TestMethod]
        public void TestCopyToArray()
        {
            var source = new CopySourceModel[2];
            source[0] = CreateSource("hello");
            source[1] = CreateSource("world");
            var target = source.MapTo<CopyTargetModel[]>();
            
            for (var i = 0; i < source.Length; i++)
            {
                Assert.AreEqual(source[i].Name, target[i].Name);
            }
        }

        [TestMethod]
        public void TestCopyIntArray()
        {
            var source = new int[1];
            source[0] = 1;

            var input = new int[1];
            var target = source.MapTo(input);

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
            var target = source.MapTo(input);

            Assert.AreEqual(input.GetHashCode(), target.GetHashCode());
            Assert.AreEqual(source.Length, target.Length);
        }

        [TestMethod]
        public void TestCopyList()
        {
            var source = new List<CopySourceModel>();
            source.Add(CreateSource("hello"));

            var input = new List<CopyTargetModel>();
            var target = source.MapTo(input);

            Assert.AreEqual(input.GetHashCode(), target.GetHashCode());
            Assert.AreEqual(source.Count, target.Count);
            Assert.AreEqual("hello", target[0].Name);
        }

        [TestMethod]
        public void TestCopyPropertyList()
        {
            var source = new CopySourceModel();
            source.SubList = new List<CopySourceSubModel>();
            source.SubList.Add(CopySourceSubModel.Create("hello"));
            source.SubList.Add(CopySourceSubModel.Create("world"));

            var input = new CopyTargetModel();
            var target = source.MapTo(input);

            Assert.AreEqual(input.GetHashCode(), target.GetHashCode());
            for (var i = 0; i < source.SubList.Count; i++)
            {
                Assert.AreEqual(source.SubList[0].Value, target.SubList[0].Value);
            }
        }

        [TestMethod]
        public void TestCopyToList()
        {
            var source = new List<CopySourceModel>();
            source.Add(CreateSource("hello"));

            var target = source.MapTo<List<CopyTargetModel>>();
            Assert.AreEqual(source.Count, target.Count);
            Assert.AreEqual("hello", target[0].Name);
        }

        [TestMethod]
        public void TestCopyDictionary()
        {
            var source = new Dictionary<string, CopySourceModel>();
            source.Add("hello", CreateSource("world"));
            var input = new Dictionary<string, CopyTargetModel>();
            var target = source.MapTo(input);

            Assert.AreEqual(input.GetHashCode(), target.GetHashCode());
            Assert.AreEqual(source.Count, target.Count);
            Assert.AreEqual("world", target["hello"].Name);
        }

        [TestMethod]
        public void TestCopyPropertyDictionary()
        {
            var source = new CopySourceModel();
            source.SubDictionary = new Dictionary<int, CopySourceSubModel>();
            source.SubDictionary.Add(1, CopySourceSubModel.Create("hello"));
            source.SubDictionary.Add(2, CopySourceSubModel.Create("world"));
            var input = new CopyTargetModel();
            var target = source.MapTo(input);

            Assert.AreEqual(input.GetHashCode(), target.GetHashCode());
            foreach (var item in source.SubDictionary)
            {
                Assert.AreEqual(item.Value.Value, target.SubDictionary[item.Key].Value);
            }
        }

        [TestMethod]
        public void TestCopyToDictionary()
        {
            var source = new Dictionary<string, CopySourceModel>();
            source.Add("hello", CreateSource("world"));
            var target = source.MapTo<Dictionary<string, CopyTargetModel>>();
            
            Assert.AreEqual(source.Count, target.Count);
            Assert.AreEqual("world", target["hello"].Name);
        }

        [TestMethod]
        public void TestCopyDiffKeyDictionary()
        {
            var source = new Dictionary<string, CopySourceModel>();
            source.Add("hello", CreateSource("world"));
            var input = new Dictionary<int, CopyTargetModel>();
            var target = source.MapTo(input);

            Assert.AreEqual(input.GetHashCode(), target.GetHashCode());
            Assert.AreEqual(0, target.Count);
        }

        [TestMethod]
        public void TestCopyItemDictionary()
        {
            var source = new ItemObject();
            source.Dic = new Dictionary<string, string>();
            source.Dic.Add("hello", "world");
            var target = source.MapTo(new ItemObject());
            Assert.AreEqual(source.Dic.Count, target.Dic.Count);
        }

        [TestMethod]
        public void TestCopyItemList()
        {
            var source = new ItemObject();
            source.List = new List<string>();
            source.List.Add("hello");
            var target = source.MapTo(new ItemObject());
            Assert.AreEqual(source.List.Count, target.List.Count);
        }

        [TestMethod]
        public void TestCopyItemArray()
        {
            var source = new ItemObject();
            source.Array = new string[1];
            source.Array[0] = "hello";
            var target = source.MapTo(new ItemObject());
            Assert.AreEqual(source.Array.Length, target.Array.Length);
            Assert.AreEqual(source.Array[0], target.Array[0]);
        }

        [TestMethod]
        public void TestCopyEnumArray()
        {
            var source = new Direction[2];
            source[0] = Direction.Up;
            source[1] = Direction.Right;
            var target = source.MapTo<Direction[]>();
            Assert.AreEqual(source.Length, target.Length);
            Assert.AreEqual(source[1], target[1]);
        }

        [TestMethod]
        public void TestCopyDicSourceNull()
        {
            var source = new ItemObject();
            var target = new ItemObject();
            target.Dic = new Dictionary<string, string>();
            source.MapTo(target);
            Assert.IsNull(target.Dic);
        }

        [TestMethod]
        public void TestCopyArraySourceNull()
        {
            var source = new ItemObject();
            var target = new ItemObject();
            target.Array = new string[0];
            source.MapTo(target);
            Assert.IsNull(target.Array);
        }

        [TestMethod]
        public void TestCopyListSourceNull()
        {
            var source = new ItemObject();
            var target = new ItemObject();
            target.List = new List<string>();
            source.MapTo(target);
            Assert.IsNull(target.List);
        }

        [TestMethod]
        public void TestCopyModelSourceNull()
        {
            var source = new ItemObject();
            var target = new ItemObject();
            target.Model = new CopySourceModel();
            source.MapTo(target);
            Assert.IsNull(target.Model);
        }

        [TestMethod]
        public void TestCopyEnumSourceNull()
        {
            var source = new ItemObject();
            var target = new ItemObject();
            target.NullEnum = ItemObject.E.B;
            source.MapTo(target);
            Assert.IsNull(target.NullEnum);
        }

        [TestMethod]
        public void TestCopyEnum()
        {
            var source = new ItemObject();
            var target = new ItemObject();
            source.Enum = ItemObject.E.B;
            source.MapTo(target);
            Assert.AreEqual(source.Enum, target.Enum);
        }

        [TestMethod]
        public void TestCopyIList()
        {
            IList<int> source = new List<int>();
            source.Add(1);
            var target = new List<int>();
            source.MapTo(target);
            Assert.AreEqual(source.Count, target.Count);
        }
    }
}
