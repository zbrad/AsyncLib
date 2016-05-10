using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZBrad.AsyncLib;
using System.Threading;
using System.Diagnostics.CodeAnalysis;
using ZBrad.AsyncLib.Collections;

namespace Tests
{
    class NodeList<T> where T : ILink, IEquatable<T>, IClone<T>
    {
        T[] values;
        public T[] Values { get { return values; } }

        LinkList<T> list = new LinkList<T>();

        public LinkList<T> List { get { return list; } }

        public NodeList(Func<T[]> values)
        {
            this.values = values();
        }

        public void InsertAtTail(int index)
        {
            var v = values[index].Clone();
            list.InsertAtTail(v);
            Assert.AreEqual<T>(v, list.Tail);
        }

        public bool IsEmpty()
        {
            Assert.AreEqual<int>(0, list.Count);
            Assert.IsNull(list.Head);
            return true;
        }

        // insert 3 items, v0, v0, v1
        public void Insert_001()
        {
            // assert we start with empty list
            Assert.IsTrue(IsEmpty());

            // insert 0 value 
            InsertAtTail(0);

            // insert 0 value 
            InsertAtTail(0);

            // add 1 value 
            InsertAtTail(1);

            // verify
            Verify_001(3);
        }

        public void Verify_001(int count)
        {
            // validate our expected list
            Assert.AreEqual<int>(count, list.Count);
            Assert.IsNotNull(list.Head);
            Assert.IsNotNull(list.Head.Prev);

            // verify items on list (from list.Head)
            Assert.AreEqual<T>(values[0], list.Head);
            Assert.IsNotNull(list.Head.Next);
            Assert.AreEqual<T>(values[0], (T)list.Head.Next);
            Assert.IsNotNull(list.Head.Next.Next);
            Assert.AreEqual<T>(values[1], (T)list.Head.Next.Next);
            Assert.IsNotNull(list.Head.Next.Next.Next);
            Assert.AreEqual<ILink>(list.Head, list.Head.Next.Next.Next);

            // verify items on list (from list.Head.Prev)
            Assert.AreEqual<T>(values[1], (T)list.Head.Prev);
            Assert.IsNotNull(list.Head.Prev.Prev);
            Assert.AreEqual<T>(values[0], (T)list.Head.Prev.Prev);
            Assert.IsNotNull(list.Head.Prev.Prev.Prev);
            Assert.AreEqual<T>(values[0], (T)list.Head.Prev.Prev.Prev);
            Assert.IsNotNull(list.Head.Prev.Prev.Prev.Prev);
            Assert.AreEqual<ILink>(list.Head.Prev, list.Head.Prev.Prev.Prev.Prev);
        }

        public void Remove_001()
        {
            // verify 001 layout
            Verify_001(3);

            // remove first item
            T item = list.RemoveFromHead();
            Assert.AreEqual<int>(2, list.Count);
            Assert.AreEqual<T>(values[0], item);

            // confirm list shape
            Assert.IsNotNull(list.Head);
            Assert.IsNotNull(list.Head.Next);
            Assert.AreEqual<ILink>(list.Head, list.Head.Next.Next);


            // test Head and Tail
            var tail = list.Head.Prev;
            Assert.AreEqual<T>(values[0], (T)list.Head);
            Assert.AreEqual<T>(values[1], (T)tail);

            // remove second item
            item = list.RemoveFromHead();
            Assert.AreEqual<int>(1, list.Count);

            // verify shape (only 1 item left)
            Assert.IsNotNull(list.Head);
            Assert.IsNotNull(list.Head.Next);
            Assert.IsNotNull(list.Head.Prev);
            Assert.AreEqual<ILink>(list.Head, list.Head.Prev);
            Assert.AreEqual<ILink>(list.Head, list.Head.Next);

            Assert.AreEqual<T>(values[0], item);
            Assert.AreEqual<T>(values[1], list.Head);

            // remove third item
            item = list.RemoveFromHead();
            Assert.AreEqual<T>(values[1], item);

            // verify queue cleaned up
            Assert.AreEqual<int>(0, list.Count);
            Assert.AreEqual<T>(default(T), list.Head);
        }
    }
}
