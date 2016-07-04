using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZBrad.AsyncLib;
using System.Threading;
using System.Diagnostics.CodeAnalysis;
using ZBrad.AsyncLib.Collections;

namespace Tests
{
    class ValueQueue<T> where T : ILink, IEquatable<T>, IComparable<T>, IClone<T>
    {
        T[] values;
        public T[] Values { get { return values; } }

        OrderedLinkQueue<T> queue = new OrderedLinkQueue<T>();

        public OrderedLinkQueue<T> Queue { get { return queue; } }

        public ValueQueue(Func<T[]> values)
        {
            this.values = values();
        }

        public void Enqueue(int index)
        {
            var v = values[index].Clone();
            queue.Enqueue(v);
        }

        public bool IsEmpty()
        {
            Assert.AreEqual<int>(0, queue.Count);
            Assert.IsNull(queue.Peek());
            return true;
        }

        // insert 3 items, v0, v0, v1
        public void Insert(params int[] indexes)
        {
            // assert we start with empty list
            Assert.IsTrue(IsEmpty());

            for (var i = 0; i < indexes.Length; i++)
                Enqueue(indexes[i]);
        }

        public void Verify(params int[] indexes)
        {
            // validate our expected list
            Assert.AreEqual<int>(indexes.Length, queue.Count);
            Assert.IsNotNull(queue.Peek());
            Assert.IsNotNull(queue.Peek().Prev);

            // verify items on list (from queue.Peek())
            var cur = queue.Peek();
            for (var i = 0; i < indexes.Length; i++)
            {
                Assert.AreEqual<T>(values[indexes[i]], cur);
                Assert.IsNotNull(cur.Next);
                cur = (T)cur.Next;
            }

            Assert.AreEqual<ILink>(queue.Peek(), cur);

            // verify items on list (from queue.Peek().Prev)
            cur = (T)queue.Peek().Prev;
            for (var i = indexes.Length - 1; i >= 0; i--)
            {
                Assert.AreEqual<T>(values[indexes[i]], cur);
                Assert.IsNotNull(cur.Prev);
                cur = (T)cur.Prev;
            }

            Assert.AreEqual<ILink>(queue.Peek().Prev, cur);
        }

        public void Remove(params int[] indexes)
        {
            for (var i = 0; i < indexes.Length; i++)
            {
                // remove item
                T item = (T)queue.Dequeue();
                Assert.AreEqual<int>(indexes.Length - 1 - i, queue.Count);
                Assert.AreEqual<T>(values[indexes[i]], item);

                if (queue.Count == 0)
                    break;

                // confirm list shape of remaining items
                var cur = (ILink)queue.Peek();
                for (var j = 0; j < queue.Count; j++)
                    cur = (T)cur.Next;
                Assert.AreEqual<ILink>(queue.Peek(), cur);
            }

            Assert.IsNull(queue.Peek());
        }
    }

    class ValueInt : Value<int>, IEquatable<ValueInt>, IComparable<ValueInt>, IClone<ValueInt>
    {
        public static ValueInt[] GetValues(int count)
        {
            ValueInt[] values = new ValueInt[count];

            // purposely put these in reverse order, they should get correctly ordered by the queue
            for (var i = 0; i < values.Length; i++)
                values[values.Length - 1 - i] = new ValueInt(i);
            return values;
        }

        public ValueInt(int i) : base(i) { }

        public bool Equals(ValueInt other) { return base.Equals(other); }

        public int CompareTo(ValueInt other) { return base.CompareTo(other); }

        public ValueInt Clone() { return new ValueInt(this.Item); }
    }
}
