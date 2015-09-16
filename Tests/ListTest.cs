using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using ZBrad.AsyncLib;

namespace Tests
{
    [ExcludeFromCodeCoverage]
    internal class ListTest<T> where T : class, IEquatable<T>, INode, new()
    {
        T[] values;
        public T[] Values { get { return values; } }

        NodeList<T> list = new NodeList<T>();
        public NodeList<T> List { get { return list; } }

        Func<T, T> copy;

        public ListTest(int count, Func<int,T> create, Func<T,T> copy)
        {
            this.copy = copy;
            values = new T[count];
            for (int i = 0; i < values.Length; i++)
                values[i] = create(i);
        }

        public void InsertAtHead(int index)
        {
            var value = this.copy(values[index]);
            list.InsertAtHead(value);
            Assert.AreEqual<T>(values[index], (T)list.Head);
        }

        public void InsertAtTail(int index)
        {
            var value = this.copy(values[index]);
            list.InsertAtTail(value);
            Assert.AreEqual<T>(values[index], (T)list.Tail);
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
            Assert.AreEqual<T>(values[0], (T)list.Head);
            Assert.IsNotNull(list.Head.Next);
            Assert.AreEqual<T>(values[0], (T)list.Head.Next);
            Assert.IsNotNull(list.Head.Next.Next);
            Assert.AreEqual<T>(values[1], (T)list.Head.Next.Next);
            Assert.IsNotNull(list.Head.Next.Next.Next);
            Assert.AreEqual<INode>(list.Head, list.Head.Next.Next.Next);

            // verify items on list (from Tail)
            Assert.AreEqual<T>(values[1], (T)list.Tail);
            Assert.IsNotNull(list.Tail.Prev);
            Assert.AreEqual<T>(values[0], (T)list.Tail.Prev);
            Assert.IsNotNull(list.Tail.Prev.Prev);
            Assert.AreEqual<T>(values[0], (T)list.Tail.Prev.Prev);
            Assert.IsNotNull(list.Tail.Prev.Prev.Prev);
            Assert.AreEqual<INode>(list.Tail, list.Tail.Prev.Prev.Prev);
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
            Assert.AreEqual<INode>(list.Head, list.Head.Next.Next);


            // test head and next
            Assert.AreEqual<T>(values[0], (T)list.Head);
            Assert.AreEqual<T>(values[1], (T)list.Head.Next);

            // remove second item
            item = list.RemoveFromHead();
            Assert.AreEqual<int>(1, list.Count);

            // verify shape (only 1 item left)
            Assert.IsNotNull(list.Head);
            Assert.IsNotNull(list.Tail);
            Assert.IsNotNull(list.Head.Prev);
            Assert.IsNotNull(list.Tail.Next);
            Assert.AreEqual<INode>(list.Head, list.Tail);
            Assert.AreEqual<INode>(list.Head, list.Head.Next);

            Assert.AreEqual<T>(values[0], item);
            Assert.AreEqual<T>(values[1], (T)list.Head);

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
