using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZBrad.AsyncLib;
using System.Threading;
using System.Diagnostics.CodeAnalysis;
using G = System.Collections.Generic;
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
            test.List.Remove(test.List.Tail.Prev.Value);
            Assert.AreEqual<int>(test.Values[0], test.List.Head.Value);
            Assert.AreEqual<int>(test.Values[1], test.List.Head.Next.Value);
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
                Assert.AreEqual<int>(test.Values[indexes[index++]], item);
            }
        }

        ListTest<int> getTest()
        {
            return new ListTest<int>(() => new int[5], (x) => x);
        }
    }
}
