using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZBrad.AsyncLib;
using System.Threading;
using System.Diagnostics.CodeAnalysis;
using ZBrad.AsyncLib.Collections;
using ZBrad.AsyncLib.Nodes;
using ZBrad.AsyncLib.Links;
namespace Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class NodeQueueOrderedTests
    {
        class NodeTest<T> where T : IEquatable<T>, IComparable<T>
        {
            T[] values;
            public T[] Values { get { return values; } }

            OrderedQueue<T> queue = new OrderedQueue<T>();
            public OrderedQueue<T> Queue { get { return queue; } }

            public NodeTest(Func<T[]> values)
            {
                this.values = values();
            }

            public void Enqueue(int index)
            {
                queue.Enqueue(values[index]);
                var peek = queue.PeekTail();
                Assert.AreEqual<T>(values[index], peek);
            }

            public bool IsEmpty()
            {
                Assert.AreEqual<int>(0, queue.Count);
                Assert.IsNull(queue.Root);
                return true;
            }

            // insert 3 items, v0, v0, v1
            public void Insert_001()
            {
                // assert we start with empty list
                Assert.IsTrue(IsEmpty());

                // insert 0 value 
                Enqueue(0);

                // insert 0 value 
                Enqueue(0);

                // add 1 value 
                Enqueue(1);

                // verify
                Verify_001(3);
            }

            public void Verify_001(int count)
            {
                // validate our expected list
                Assert.AreEqual<int>(count, queue.Count);
                Assert.IsNotNull(queue.Root);
                Assert.IsNotNull(queue.Root.Prev);

                // verify items on list (from Root)
                var root = queue.Root;
                Assert.AreEqual<T>(values[0], root.Value);
                Assert.IsNotNull(queue.Root.Next);
                Assert.AreEqual<T>(values[0], root.Next.Value);
                Assert.IsNotNull(queue.Root.Next.Next);
                Assert.AreEqual<T>(values[1], root.Next.Next.Value);
                Assert.IsNotNull(queue.Root.Next.Next.Next);
                Assert.AreEqual<ILink>(root, root.Next.Next.Next);

                // verify items on list (from Root.Prev)
                Assert.AreEqual<T>(values[1], root.Prev.Value);
                Assert.IsNotNull(queue.Root.Prev.Prev);
                Assert.AreEqual<T>(values[0], root.Prev.Prev.Value);
                Assert.IsNotNull(queue.Root.Prev.Prev.Prev);
                Assert.AreEqual<T>(values[0], root.Prev.Prev.Prev.Value);
                Assert.IsNotNull(queue.Root.Prev.Prev.Prev.Prev);
                Assert.AreEqual<ILink>(queue.Root.Prev, queue.Root.Prev.Prev.Prev.Prev);
            }

            public void Remove_001()
            {
                // verify 001 layout
                Verify_001(3);

                // remove first item
                T item = (T)queue.Dequeue();
                Assert.AreEqual<int>(2, queue.Count);
                Assert.AreEqual<T>(values[0], item);

                // confirm list shape
                Assert.IsNotNull(queue.Root);
                Assert.IsNotNull(queue.Root.Next);
                Assert.AreEqual<ILink>(queue.Root, queue.Root.Next.Next);


                // test Head and Tail
                Assert.AreEqual<T>(values[0], queue.PeekHead());
                Assert.AreEqual<T>(values[1], queue.PeekTail());

                // remove second item
                item = (T)queue.Dequeue();
                Assert.AreEqual<int>(1, queue.Count);

                // verify shape (only 1 item left)
                Assert.IsNotNull(queue.Root);
                Assert.IsNotNull(queue.Root.Next);
                Assert.IsNotNull(queue.Root.Prev);
                Assert.AreEqual<ILink>(queue.Root, queue.Root.Prev);
                Assert.AreEqual<ILink>(queue.Root, queue.Root.Next);

                Assert.AreEqual<T>(values[0], item);
                Assert.AreEqual<T>(values[1], queue.PeekHead());

                // remove third item
                item = queue.Dequeue();
                Assert.AreEqual<int>(0, queue.Count);
                Assert.AreEqual<T>(values[1], item);

                // verify Root/Root.Prev cleaned up
                Assert.AreEqual<T>(default(T), queue.PeekHead());
                Assert.AreEqual<T>(default(T), queue.PeekTail());
                Assert.IsNull(queue.Root);
            }
        }

        static int[] GetValues(int count)
        {
            int[] values = new int[count];
            for (var i = 0; i < values.Length; i++)
                values[i] = i;
            return values;
        }

        [TestMethod]
        public void Insert()
        {
            var test = new NodeTest<int>(() => GetValues(5));
            test.Insert_001();
        }

        [TestMethod]
        public void Remove()
        {
            var test = new NodeTest<int>(() => GetValues(5));
            test.Insert_001();
            test.Remove_001();
        }

        [TestMethod]
        public void Enumerate()
        {
            var test = new NodeTest<int>(() => GetValues(5));
            test.Insert_001();

            int index = 0;
            int[] indexes = { 0, 0, 1 };
            foreach (var item in test.Queue)
            {
                Assert.AreEqual<int>(test.Values[indexes[index++]], item);
            }
        }
    }
}
