using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZBrad.AsyncLib;
using System.Threading;
using System.Diagnostics.CodeAnalysis;

namespace Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class NodeListTests
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
        public void RemoveMiddle()
        {
            var test = getTest();
            test.Insert_001();
            test.List.Remove((ItemNode<int>)test.List.Tail.Prev);
            Assert.AreEqual<ItemNode<int>>(test.Values[0], (ItemNode<int>)test.List.Head);
            Assert.AreEqual<ItemNode<int>>(test.Values[1], (ItemNode<int>)test.List.Head.Next);
        }

        [TestMethod]
        public void Enumerate()
        {
            var test = getTest();
            test.Insert_001();

            int index = 0;
            int[] indexes = { 0, 0, 1 };
            foreach (var item in test.List)
            {
                Assert.AreEqual<ItemNode<int>>(test.Values[indexes[index++]], item);
            }
        }

        ListTest<ItemNode<int>> getTest()
        {
            return new ListTest<ItemNode<int>>(5,
                (i) => new ItemNode<int> { Item = i },
                (t) => new ItemNode<int> { Item = t.Item });
        }
    }
}
