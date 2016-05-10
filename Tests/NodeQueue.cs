using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZBrad.AsyncLib;
using System.Threading;
using System.Diagnostics.CodeAnalysis;
using ZBrad.AsyncLib.Collections;

namespace Tests
{
    public interface IClone<T>
    {
        T Clone();
    }

    class NodeQueue<T> where T : ILink, IEquatable<T>, IClone<T>
    {
        T[] values;
        public T[] Values { get { return values; } }

        LinkQueue<T> queue = new LinkQueue<T>();

        public LinkQueue<T> Queue { get { return queue; } }

        public NodeQueue(Func<T[]> values)
        {
            this.values = values();
        }

        public void Enqueue(int index)
        {
            var v = values[index].Clone();
            queue.Enqueue(v);
            var tail = (T)queue.Peek().Prev;
            Assert.AreEqual<T>(v, tail);
        }

        public bool IsEmpty()
        {
            Assert.AreEqual<int>(0, queue.Count);
            Assert.IsNull(queue.Peek());
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
            Assert.IsNotNull(queue.Peek());
            Assert.IsNotNull(queue.Peek().Prev);

            // verify items on list (from queue.Peek())
            Assert.AreEqual<T>(values[0], queue.Peek());
            Assert.IsNotNull(queue.Peek().Next);
            Assert.AreEqual<T>(values[0], (T)queue.Peek().Next);
            Assert.IsNotNull(queue.Peek().Next.Next);
            Assert.AreEqual<T>(values[1], (T)queue.Peek().Next.Next);
            Assert.IsNotNull(queue.Peek().Next.Next.Next);
            Assert.AreEqual<ILink>(queue.Peek(), queue.Peek().Next.Next.Next);

            // verify items on list (from queue.Peek().Prev)
            Assert.AreEqual<T>(values[1], (T)queue.Peek().Prev);
            Assert.IsNotNull(queue.Peek().Prev.Prev);
            Assert.AreEqual<T>(values[0], (T)queue.Peek().Prev.Prev);
            Assert.IsNotNull(queue.Peek().Prev.Prev.Prev);
            Assert.AreEqual<T>(values[0], (T)queue.Peek().Prev.Prev.Prev);
            Assert.IsNotNull(queue.Peek().Prev.Prev.Prev.Prev);
            Assert.AreEqual<ILink>(queue.Peek().Prev, queue.Peek().Prev.Prev.Prev.Prev);
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
            Assert.IsNotNull(queue.Peek());
            Assert.IsNotNull(queue.Peek().Next);
            Assert.AreEqual<ILink>(queue.Peek(), queue.Peek().Next.Next);


            // test Head and Tail
            var tail = queue.Peek().Prev;
            Assert.AreEqual<T>(values[0], (T)queue.Peek());
            Assert.AreEqual<T>(values[1], (T)tail);

            // remove second item
            item = (T)queue.Dequeue();
            Assert.AreEqual<int>(1, queue.Count);

            // verify shape (only 1 item left)
            Assert.IsNotNull(queue.Peek());
            Assert.IsNotNull(queue.Peek().Next);
            Assert.IsNotNull(queue.Peek().Prev);
            Assert.AreEqual<ILink>(queue.Peek(), queue.Peek().Prev);
            Assert.AreEqual<ILink>(queue.Peek(), queue.Peek().Next);

            Assert.AreEqual<T>(values[0], item);
            Assert.AreEqual<T>(values[1], queue.Peek());

            // remove third item
            item = queue.Dequeue();
            Assert.AreEqual<T>(values[1], item);

            // verify queue cleaned up
            Assert.AreEqual<int>(0, queue.Count);
            Assert.AreEqual<T>(default(T), queue.Peek());
        }
    }

    class NodeInt : Value<int>, IEquatable<NodeInt>, IClone<NodeInt>
    {
        public static NodeInt[] GetValues(int count)
        {
            NodeInt[] values = new NodeInt[count];
            for (var i = 0; i < values.Length; i++)
                values[i] = new NodeInt(i);
            return values;
        }

        public NodeInt(int i) : base(i) { }

        public bool Equals(NodeInt other) { return base.Equals(other); }

        public NodeInt Clone() { return new NodeInt(this.Item); }
    }

}
