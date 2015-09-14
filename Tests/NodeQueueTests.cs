using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZBrad.AsyncLib;
using System.Threading;
namespace Tests
{
    [TestClass]
    public class NodeQueueTests
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

            NodeQueue<T> queue = new NodeQueue<T>();
            public NodeQueue<T> Queue { get { return queue; } }

            public NodeTest(int count)
            {
                values = new T[count];
                for (int i = 0; i < values.Length; i++)
                    values[i] = new T() { Value = i };
            }

            public void Enqueue(int index)
            {
                var value = new T() { Value = values[index].Value };
                queue.Enqueue(value);
                Assert.AreEqual<T>(values[index], queue.PeekTail());
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
                T item = queue.Dequeue();
                Assert.AreEqual<int>(2, queue.Count);
                Assert.AreEqual<T>(values[0], item);

                // confirm list shape
                Assert.IsNotNull(queue.Root);
                Assert.IsNotNull(queue.Root.Next);
                Assert.AreEqual<INode>(queue.Root, queue.Root.Next.Next);


                // test Head and Tail
                Assert.AreEqual<T>(values[0], queue.PeekHead());
                Assert.AreEqual<T>(values[1], queue.PeekTail());

                // remove second item
                item = queue.Dequeue();
                Assert.AreEqual<int>(1, queue.Count);

                // verify shape (only 1 item left)
                Assert.IsNotNull(queue.Root);
                Assert.IsNotNull(queue.Root.Next);
                Assert.IsNotNull(queue.Root.Prev);
                Assert.AreEqual<INode>(queue.Root, queue.Root.Prev);
                Assert.AreEqual<INode>(queue.Root, queue.Root.Next);

                Assert.AreEqual<T>(values[0], item);
                Assert.AreEqual<T>(values[1], queue.PeekHead());

                // remove third item
                item = queue.Dequeue();
                Assert.AreEqual<int>(0, queue.Count);
                Assert.AreEqual<T>(values[1], item);

                // verify Root/Root.Prev cleaned up
                Assert.IsNull(queue.PeekHead());
                Assert.IsNull(queue.PeekTail());
                Assert.IsNull(queue.Root);
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
        public void Enumerate()
        {
            var test = new NodeTest<IntClass>(5);
            test.Insert_001();

            int index = 0;
            int[] indexes = { 0, 0, 1 };
            foreach (var item in test.Queue)
            {
                Assert.AreEqual<IntClass>(test.Values[indexes[index++]], item);
            }
        }
    }
}
