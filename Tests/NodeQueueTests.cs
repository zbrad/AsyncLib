using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZBrad.AsyncLib;
using System.Threading;
using System.Diagnostics.CodeAnalysis;
using ZBrad.AsyncLib.Nodes;

namespace Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class NodeQueueTests
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
                Assert.AreEqual<int>(test.Values[indexes[index++]], item);
            }
        }

        NodeQueueOrderedTest<int> getTest()
        {
            return new NodeQueueOrderedTest<int>(() => new int[5], (x) => x);
        }
    }
}
