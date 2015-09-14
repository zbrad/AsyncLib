using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZBrad.AsyncLib;
using System.Threading;
namespace Tests
{
    [TestClass]
    public class NodeListTests
    {
        interface IValue
        {
            int Value { get; set; }
        }

        class IntClass : INode, IValue
        {
            static int sequence;
            public int Id = Interlocked.Increment(ref sequence);
            public int Value { get; set; }
            public INode Next { get; set; }
            public INode Prev { get; set; }

            public bool Equals(IntClass other)
            {
                return Value == other.Value;
            }

            public override bool Equals(object other)
            {
                if (other != null && other is IntClass)
                    return this.Equals((IntClass)other);
                return false;
            }

            public override int GetHashCode()
            {
                return this.Value.GetHashCode();
            }

            public override string ToString()
            {
                return "{ Value=" + Value + " Id=" + Id + " }";
            }
        }

        class NodeTest<T> where T : class, IValue, INode, new()
        {
            T[] values;
            public T[] Values { get { return values; } }

            NodeList<T> list = new NodeList<T>();
            public NodeList<T> List { get { return list; } }

            public NodeTest(int count)
            {
                values = new T[count];
                for (int i = 0; i < values.Length; i++)
                    values[i] = new T() { Value = i };
            }

            public void InsertAtHead(int index)
            {
                var value = new T() { Value = values[index].Value };
                list.InsertAtHead(value);
                Assert.AreEqual<T>(values[index], (T)list.Head);
            }

            public void InsertAtTail(int index)
            {
                var value = new T() { Value = values[index].Value };
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


        [TestMethod]
        public void Insert()
        {
            var test = new NodeTest<IntClass>(5);
            test.Insert_001();
        }

        [TestMethod]
        public void Remove()
        {
            var test = new NodeTest<IntClass>(5);
            test.Insert_001();
            test.Remove_001();
        }

        [TestMethod]
        public void RemoveMiddle()
        {
            var test = new NodeTest<IntClass>(5);
            test.Insert_001();
            test.List.Remove((IntClass)test.List.Tail.Prev);
            Assert.AreEqual<IntClass>(test.Values[0], (IntClass)test.List.Head);
            Assert.AreEqual<IntClass>(test.Values[1], (IntClass)test.List.Head.Next);
        }

        [TestMethod]
        public void Enumerate()
        {
            var test = new NodeTest<IntClass>(5);
            test.Insert_001();

            int index = 0;
            int[] indexes = { 0, 0, 1 };
            foreach (var item in test.List)
            {
                Assert.AreEqual<IntClass>(test.Values[indexes[index++]], item);
            }
        }
    }
}
