using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oldmansoft.ClassicDomain.Util;

namespace UnitTest.ClassicDomain.Util
{
    [TestClass]
    public class UtPaging
    {
        [TestMethod]
        public void TestListPageing()
        {
            var source = new List<Data>();
            for (var i = 0; i < 1000; i++)
            {
                source.Add(new Data { Id = i });
            }
            int count;
            var list = source.AsQueryable().Paging().Where(o => o.Id > 10).OrderBy(o => o.Id).Size(20).GetResult(out count, 1);
            Assert.AreEqual(1000 - 11, count, "总数");
            Assert.AreEqual(20, list.Length, "长度");
            for (var i = 0; i < list.Length; i++)
            {
                Assert.AreEqual(i + 11, list[i].Id, string.Format("第 {0} 位", i));
            }
        }

        [TestMethod]
        public void TestMoreResultPageing()
        {
            var source = new List<Data>();
            for (var i = 0; i < 1000; i++)
            {
                source.Add(new Data { Id = i });
            }
            int count;
            var where = source.AsQueryable().Paging().Where(o => o.Id > 10);
            var list = where.OrderBy(o => o.Id).Size(20).GetResult(out count, 1);
            Assert.AreEqual(1000 - 11, count, "比较一总数");
            Assert.AreEqual(20, list.Length, "比较一长度");
            for (var i = 0; i < list.Length; i++)
            {
                Assert.AreEqual(i + 11, list[i].Id, string.Format("比较一第 {0} 位", i));
            }

            list = where.Where(o => o.Id < 100).OrderByDescending(o => o.Id).Size(20).GetResult(out count, 1);
            Assert.AreEqual(100 - 11, count, "比较二总数");
            Assert.AreEqual(20, list.Length, "比较二长度");
            for (var i = 0; i < list.Length; i++)
            {
                Assert.AreEqual(99 - i, list[i].Id, string.Format("比较二第 {0} 位", i));
            }
        }

        class Data
        {
            public int Id { get; set; }
        }
    }
}
