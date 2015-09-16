using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using ZBrad.AsyncLib;

namespace Tests
{
    [ExcludeFromCodeCoverage]
    internal class QueueTest<T> where T : class, INodeComparable<T>, IEquatable<T>, new()
    {
        T[] values;
        public T[] Values { get { return values; } }

        NodeQueueOrdered<T> queue = new NodeQueueOrdered<T>();
        public NodeQueueOrdered<T> Queue { get { return queue; } }

        Func<T, T> copy;

        public QueueTest(int count, Func<int, T> create, Func<T, T> copy)
        {
            this.copy = copy;
            this.values = new T[count];
            for (var i = 0; i < this.values.Length; i++)
                this.values[i] = create(i);
        }

        public void Enqueue(int index)
        {
            var value = this.copy(values[index]);
            queue.Enqueue(value);
            Assert.AreEqual<T>(values[index], (T)queue.PeekTail());
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
            Verify_001();
        }

        public void Verify_001()
        {
            // validate our expected list
            Assert.AreEqual<int>(3, queue.Count);
            Assert.IsNotNull(queue.Root);
            Assert.IsNotNull(queue.Root.Prev);

            // verify items on list (from Root)
            Assert.AreEqual<T>(values[0], (T)queue.Root);
            Assert.IsNotNull(queue.Root.Next);
            Assert.AreEqual<T>(values[0], (T)queue.Root.Next);
            Assert.IsNotNull(queue.Root.Next.Next);
            Assert.AreEqual<T>(values[1], (T)queue.Root.Next.Next);
            Assert.IsNotNull(queue.Root.Next.Next.Next);
            Assert.AreEqual<INode>(queue.Root, queue.Root.Next.Next.Next);

            // verify items on list (from Root.Prev)
            Assert.AreEqual<T>(values[1], (T)queue.Root.Prev);
            Assert.IsNotNull(queue.Root.Prev.Prev);
            Assert.AreEqual<T>(values[0], (T)queue.Root.Prev.Prev);
            Assert.IsNotNull(queue.Root.Prev.Prev.Prev);
            Assert.AreEqual<T>(values[0], (T)queue.Root.Prev.Prev.Prev);
            Assert.IsNotNull(queue.Root.Prev.Prev.Prev.Prev);
            Assert.AreEqual<INode>(queue.Root.Prev, queue.Root.Prev.Prev.Prev.Prev);
        }

        public void Remove_001()
        {
            // verify 001 layout
            Verify_001();

            // remove first item
            T item = (T)queue.Dequeue();
            Assert.AreEqual<int>(2, queue.Count);
            Assert.AreEqual<T>(values[0], item);

            // confirm list shape
            Assert.IsNotNull(queue.Root);
            Assert.IsNotNull(queue.Root.Next);
            Assert.AreEqual<INode>(queue.Root, queue.Root.Next.Next);


            // test Head and Tail
            Assert.AreEqual<T>(values[0], (T)queue.PeekHead());
            Assert.AreEqual<T>(values[1], (T)queue.PeekTail());

            // remove second item
            item = (T)queue.Dequeue();
            Assert.AreEqual<int>(1, queue.Count);

            // verify shape (only 1 item left)
            Assert.IsNotNull(queue.Root);
            Assert.IsNotNull(queue.Root.Next);
            Assert.IsNotNull(queue.Root.Prev);
            Assert.AreEqual<INode>(queue.Root, queue.Root.Prev);
            Assert.AreEqual<INode>(queue.Root, queue.Root.Next);

            Assert.AreEqual<T>(values[0], item);
            Assert.AreEqual<T>(values[1], (T)queue.PeekHead());

            // remove third item
            item = (T)queue.Dequeue();
            Assert.AreEqual<int>(0, queue.Count);
            Assert.AreEqual<T>(values[1], item);

            // verify Root/Root.Prev cleaned up
            Assert.IsNull(queue.PeekHead());
            Assert.IsNull(queue.PeekTail());
            Assert.IsNull(queue.Root);
        }
    }
}
