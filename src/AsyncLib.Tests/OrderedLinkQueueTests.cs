﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZBrad.AsyncLib;
using System.Threading;
using System.Diagnostics.CodeAnalysis;
using ZBrad.AsyncLib.Collections;

namespace Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class OrderedLinkQueueTests
    {

        [TestMethod]
        public void Insert()
        {
            var test = new ValueQueue<ValueInt>(() => ValueInt.GetValues(5));
            test.Insert(0, 4, 0);
            test.Verify(4, 0, 0);
        }

        [TestMethod]
        public void Remove()
        {
            var test = new ValueQueue<ValueInt>(() => ValueInt.GetValues(5));
            test.Insert(0, 4, 0);
            test.Verify(4, 0, 0);
            test.Remove(4, 0, 0);
        }

        [TestMethod]
        public void Enumerate()
        {
            var test = new ValueQueue<ValueInt>(() => ValueInt.GetValues(5));
            test.Insert(0, 4, 0);
            test.Verify(4, 0, 0);

            int index = 0;
            int[] indexes = { 4, 0, 0 };
            foreach (var item in test.Queue)
            {
                Assert.AreEqual<ValueInt>(test.Values[indexes[index++]], item);
            }
        }
    }
}
