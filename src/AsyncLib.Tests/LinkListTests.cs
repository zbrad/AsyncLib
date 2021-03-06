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
    public class LinkListTests
    {
        [TestMethod]
        public void Insert()
        {
            var test = new NodeList<NodeInt>(() => NodeInt.GetValues(5));
            test.Insert_001();
        }

        [TestMethod]
        public void Remove()
        {
            var test = new NodeList<NodeInt>(() => NodeInt.GetValues(5));
            test.Insert_001();
            test.Remove_001();
        }

        [TestMethod]
        public void Enumerate()
        {
            var test = new NodeList<NodeInt>(() => NodeInt.GetValues(5));
            test.Insert_001();

            int index = 0;
            int[] indexes = { 0, 0, 1 };
            foreach (var item in test.List)
            {
                Assert.AreEqual<NodeInt>(test.Values[indexes[index++]], item);
            }
        }
    }
}
