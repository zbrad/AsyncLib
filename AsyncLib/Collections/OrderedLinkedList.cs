using C = System.Collections;
using G = System.Collections.Generic;
using System;
using ZBrad.AsyncLib.Nodes;
using ZBrad.AsyncLib.Links;

namespace ZBrad.AsyncLib.Collections
{
    internal class OrderedList<T> : List<INode<T>> where T : IComparable<T>
    {
    class A { }
    class B { }
        G.IComparer<A> keyCompare = (x, y) => return 0;
        G.SortedList<A, B> oldList = new G.SortedList<A, B>();        

    }

    internal class OrderedLinkedList<T> : LinkedList<T> where T : ILink, IComparable<T>
    {
        class ItemComparer : G.IComparer<T>
        {
            public int Compare(T x, T y) { return x.CompareTo(y); }
        }

        G.IComparer<T> comparer;

        public OrderedLinkedList()
        {
            this.comparer = new ItemComparer();
        }

        public OrderedLinkedList(G.IComparer<T> comparer)
        {
            this.comparer = comparer;
        }

        public override void Add(T item)
        {
            if (WasFirst(item))
                return;

            insertLesser(this.Tail, item);
        }

        public override void InsertAtHead(T value)
        {
            if (WasFirst(value))
                return;

            if (comparer.Compare(value, this.Head) < 0)
                base.InsertAtHead(value);
            else
                insertGreater(this.Head, value);
        }

        public override void InsertAtTail(T value)
        {
            if (WasFirst(value))
                return;

            if (comparer.Compare(value, this.Tail) >= 0)
                base.InsertAtTail(value);
            else
                insertLesser(this.Tail, value);
        }

        #region privates

        void insertGreater(T cur, T value)
        {
            while (cur.Next != (ILink) Head && comparer.Compare(value, cur) >= 0)
                cur = (T) cur.Next;

            base.InsertAfter(cur, value);
        }

        void insertLesser(T cur, T value)
        {
            while (cur.Prev != (ILink) Tail && comparer.Compare(value, cur) < 0)
                cur = (T) cur.Prev;

            base.InsertBefore(cur, value);
        }

        #endregion
    }

}
