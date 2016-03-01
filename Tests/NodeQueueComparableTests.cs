using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZBrad.AsyncLib;
using System.Threading;
using System.Diagnostics.CodeAnalysis;

namespace Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class NodeQueueComparableTests
    {
        [TestMethod]
        public void Insert()
        {
            var test = getTest();
            test.Insert_001();
        }

        [TestMethod]
        public void Remove()
        {
            var test = getTest();
            test.Insert_001();
            test.Remove_001();
        }

        [TestMethod]
        public void Enumerate()
        {
            var test = getTest();
            test.Insert_001();

            int index = 0;
            int[] indexes = { 0, 0, 1 };
            foreach (var item in test.Queue)
            {
                Assert.AreEqual<ItemNode<int>>(test.Values[indexes[index++]], item);
            }
        }

        NodeQueueOrderedTest<ItemNode<int>> getTest()
        {
            return new NodeQueueOrderedTest<ItemNode<int>>(5,
                (i) => new ItemNode<int> { Item = i },
                (t) => new ItemNode<int> { Item = t.Item });
        }
    }
}
