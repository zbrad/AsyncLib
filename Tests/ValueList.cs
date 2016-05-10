using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZBrad.AsyncLib;
using System.Threading;
using System.Diagnostics.CodeAnalysis;
using ZBrad.AsyncLib.Collections;

namespace Tests
{
    class ValueList<T> where T : ILink, IEquatable<T>, IComparable<T>, IClone<T>
    {
        T[] values;
        public T[] Values { get { return values; } }

        OrderedLinkList<T> list = new OrderedLinkList<T>();

        public OrderedLinkList<T> List { get { return list; } }

        public ValueList(Func<T[]> values)
        {
            this.values = values();
        }

        public void InsertAtTail(int index)
        {
            var v = values[index].Clone();
            list.InsertAtTail(v);
        }

        public bool IsEmpty()
        {
            Assert.AreEqual<int>(0, list.Count);
            Assert.IsNull(list.Head);
            return true;
        }

        // insert 3 items, v0, v0, v1
        public void Insert(params int[] indexes)
        {
            // assert we start with empty list
            Assert.IsTrue(IsEmpty());

            for (var i = 0; i < indexes.Length; i++)
                InsertAtTail(indexes[i]);
        }

        public void Verify(params int[] indexes)
        {
            // validate our expected list
            Assert.AreEqual<int>(indexes.Length, list.Count);
            Assert.IsNotNull(list.Head);
            Assert.IsNotNull(list.Head.Prev);

            // verify items on list (from list.Head)
            var cur = list.Head;
            for (var i = 0; i < indexes.Length; i++)
            {
                Assert.AreEqual<T>(values[indexes[i]], cur);
                Assert.IsNotNull(cur.Next);
                cur = (T)cur.Next;
            }

            Assert.AreEqual<ILink>(list.Head, cur);

            // verify items on list (from list.Head.Prev)
            cur = (T)list.Head.Prev;
            for (var i = indexes.Length - 1; i >= 0; i--)
            {
                Assert.AreEqual<T>(values[indexes[i]], cur);
                Assert.IsNotNull(cur.Prev);
                cur = (T)cur.Prev;
            }

            Assert.AreEqual<ILink>(list.Head.Prev, cur);
        }

        public void Remove(params int[] indexes)
        {
            for (var i = 0; i < indexes.Length; i++)
            {
                T item = list.RemoveFromHead();
                Assert.AreEqual<int>(indexes.Length - 1 - i, list.Count);
                Assert.AreEqual<T>(values[indexes[i]], item);

                // confirm prev/next
                if (list.Count == 0)
                    break;

                Assert.IsNotNull((ILink)list.Head);
                Assert.IsNotNull((ILink)list.Tail);

                // confirm list shape
                var cur = list.Head;
                for (var j = 0; j < list.Count; j++)
                    cur = (T)cur.Next;
                Assert.AreEqual<ILink>(list.Head, cur);
            }

            // validate empty
            Assert.IsNull((ILink)list.Head);
            Assert.IsNull((ILink)list.Tail);
        }
    }
}
