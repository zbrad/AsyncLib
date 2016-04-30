using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using ZBrad.AsyncLib;
using ZBrad.AsyncLib.Collections;
using ZBrad.AsyncLib.Links;

namespace Tests
{
    [ExcludeFromCodeCoverage]
    internal class ListTest<T> where T : IEquatable<T>, IComparable<T>
    {
        T[] values;
        public T[] Values { get { return values; } }

        LinkedList<T> list = new LinkedList<T>();
        public LinkedList<T> List { get { return list; } }

        Func<T, T> copy;

        public ListTest(Func<T[]> create, Func<T,T> copy)
        {
            this.copy = copy;
            this.values = create();
        }

        public void InsertAtHead(int index)
        {
            var value = this.copy(values[index]);
            list.InsertAtHead(value);
            Assert.AreEqual<T>(values[index], list.Head.Value);
        }

        public void InsertAtTail(int index)
        {
            var value = this.copy(values[index]);
            list.InsertAtTail(value);
            Assert.AreEqual<T>(values[index], list.Tail.Value);
        }

        public bool IsEmpty()
        {
            Assert.AreEqual<int>(0, list.Count);
            Assert.IsNull(list.Head);
            Assert.IsNull(list.Tail);

            return true;
        }

        // insert 3 items, v0, v0, v1
        public void Insert_001()
        {
            // assert we start with empty list
            Assert.IsTrue(IsEmpty());

            // insert 0 value at head
            InsertAtHead(0);

            // insert 0 value at tail
            InsertAtTail(0);

            // add 1 value at tail
            InsertAtTail(1);

            // verify
            Verify_001();
        }

        public void Verify_001()
        {
            // validate our expected list
            Assert.AreEqual<int>(3, list.Count);
            Assert.IsNotNull(list.Head);
            Assert.IsNotNull(list.Tail);

            // verify items on list (from Head)
            Assert.AreEqual<T>(values[0], list.Head.Value);
            Assert.IsNotNull(list.Head.Next);
            Assert.AreEqual<T>(values[0], list.Head.Next.Value);
            Assert.IsNotNull(list.Head.Next.Next);
            Assert.AreEqual<T>(values[1], list.Head.Next.Next.Value);
            Assert.IsNotNull(list.Head.Next.Next.Next);
            Assert.AreEqual<ILink>(list.Head, list.Head.Next.Next.Next);

            // verify items on list (from Tail)
            Assert.AreEqual<T>(values[1], list.Tail.Value);
            Assert.IsNotNull(list.Tail.Prev);
            Assert.AreEqual<T>(values[0], list.Tail.Prev.Value);
            Assert.IsNotNull(list.Tail.Prev.Prev);
            Assert.AreEqual<T>(values[0], list.Tail.Prev.Prev.Value);
            Assert.IsNotNull(list.Tail.Prev.Prev.Prev);
            Assert.AreEqual<ILink>(list.Tail, list.Tail.Prev.Prev.Prev);
        }

        public void Remove_001()
        {
            // verify 001 layout
            Verify_001();

            // remove first item
            T item = list.RemoveFromHead();
            Assert.AreEqual<int>(2, list.Count);
            Assert.AreEqual<T>(values[0], item);

            // confirm list shape
            Assert.IsNotNull(list.Head);
            Assert.IsNotNull(list.Head.Next);
            Assert.AreEqual<ILink>(list.Head, list.Head.Next.Next);


            // test head and next
            Assert.AreEqual<T>(values[0], list.Head.Value);
            Assert.AreEqual<T>(values[1], list.Head.Next.Value);

            // remove second item
            item = list.RemoveFromHead();
            Assert.AreEqual<int>(1, list.Count);

            // verify shape (only 1 item left)
            Assert.IsNotNull(list.Head);
            Assert.IsNotNull(list.Tail);
            Assert.IsNotNull(list.Head.Prev);
            Assert.IsNotNull(list.Tail.Next);
            Assert.AreEqual<ILink>(list.Head, list.Tail);
            Assert.AreEqual<ILink>(list.Head, list.Head.Next);

            Assert.AreEqual<T>(values[0], item);
            Assert.AreEqual<T>(values[1], list.Head.Value);

            // remove third item
            item = list.RemoveFromHead();
            Assert.AreEqual<int>(0, list.Count);
            Assert.AreEqual<T>(values[1], item);

            // verify head/tail cleaned up
            Assert.IsNull(list.Head);
            Assert.IsNull(list.Tail);
        }
    }
}
