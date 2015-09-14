using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZBrad.AsyncLib;
using System.Threading;
namespace Tests
{
    [TestClass]
    public class NodeQueueComparableTests
    {

        class ItemNode : IValue<int>, IEquatable<ItemNode>, IComparable<ItemNode>, INodeComparable<ItemNode>
        {
            static int sequence;
            public int Id = Interlocked.Increment(ref sequence);
            public int Item { get; set; }
            public INode Next { get; set; }
            public INode Prev { get; set; }

            public int CompareTo(INodeComparable<ItemNode> other)
            {
                if (other == null)
                    return -1;

                return CompareTo(other as ItemNode);
            }

            public int CompareTo(ItemNode other)
            {
                if (other == null)
                    return -1;

                return Item.CompareTo(other.Item);
            }

            public bool Equals(ItemNode other)
            {
                if (other == null)
                    return false;

                return Item == other.Item;
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as ItemNode);
            }

            public override int GetHashCode()
            {
                return Item;
            }

            //public int CompareTo(INodeComparable<IOrdered<int>> other)
            //{
            //    throw new NotImplementedException();
            //}

            //public int CompareTo(INodeComparable<N> other)
            //{
            //    if (other == null)
            //        return -1;

            //    return CompareTo(other as IOrdered<N>);
            //}

            //public int CompareTo(INodeComparable<ItemNode<N>> other)
            //{
            //    if (other == null)
            //        return -1;

            //    return CompareTo(other as IOrdered<N>);
            //}

            //public int CompareTo(INodeComparable<int> other)
            //{
            //    throw new NotImplementedException();
            //}

            //public int CompareTo(IOrdered<int> other)
            //{
            //    throw new NotImplementedException();
            //}

            //public int CompareTo(IOrdered<N> other)
            //{
            //    if (other == null)
            //        return -1;

            //    return Item.CompareTo(other.Item);
            //}

            //public int CompareTo(INodeComparable<IOrdered<N>> other)
            //{
            //    if (other == null)
            //        return -1;

            //    return CompareTo(other as IOrdered<N>);
            //}

            //public int CompareTo(IOrdered<N> other)
            //{
            //    if (other == null)
            //        return -1;

            //    return Item.CompareTo(other.Item);
            //}

            //public int CompareTo(INodeComparable<IOrdered<N>> other)
            //{
            //    if (other == null)
            //        return -1;

            //    return CompareTo(other as IOrdered<N>);
            //}

            //public int CompareTo(INodeComparable<N> other)
            //{
            //    if (other == null)
            //        return -1;

            //    return CompareTo(other as IOrdered<N>);
            //}

            public override string ToString()
            {
                return "{ Value=" + Item + " Id=" + Id + " }";
            }
        }

        class NodeTest<T> where T : class, INodeComparable<T>, IValue<int>, new()
        {
            T[] values;
            public T[] Values { get { return values; } }

            NodeQueueOrdered<T> queue = new NodeQueueOrdered<T>();
            public NodeQueueOrdered<T> Queue { get { return queue; } }

            public NodeTest(int len)
            {
                this.values = new T[len];
                for (var i = 0; i < this.values.Length; i++)
                    this.values[i] = new T { Item = i };
            }

            public void Enqueue(int index)
            {
                var value = new T { Item = values[index].Item };
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

        static ItemNode[] ItemValues = GetValues();
        static ItemNode[] GetValues()
        {
            ItemNode[] values = new ItemNode[5];
            for (var i = 0; i < values.Length; i++)
                values[i] = new ItemNode() { Item = i };
            return values;
        }

        [TestMethod]
        public void Insert()
        {
            var test = new NodeTest<ItemNode>(5);
            test.Insert_001();
        }

        [TestMethod]
        public void Remove()
        {
            var test = new NodeTest<ItemNode>(5);
            test.Insert_001();
            test.Remove_001();
        }

        [TestMethod]
        public void Enumerate()
        {
            var test = new NodeTest<ItemNode>(5);
            test.Insert_001();

            int index = 0;
            int[] indexes = { 0, 0, 1 };
            foreach (var item in test.Queue)
            {
                Assert.AreEqual<ItemNode>(test.Values[indexes[index++]], item);
            }
        }
    }
}
