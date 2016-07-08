using C = System.Collections;
using G = System.Collections.Generic;
using System;

namespace ZBrad.AsyncLib.Collections
{
    internal class OrderedLinkList<T> : LinkList<T> where T : ILink, IEquatable<T>, IComparable<T>
    {
        class ItemComparer : G.IComparer<T>
        {
            public int Compare(T x, T y) { return x.CompareTo(y); }
        }

        G.IComparer<T> comparer;

        public OrderedLinkList()
        {
            this.comparer = new ItemComparer();
        }

        public OrderedLinkList(G.IComparer<T> comparer)
        {
            this.comparer = comparer;
        }

        public override void InsertAtHead(T value)
        {
                InsertAt(this.Head, value);
        }

        public override void InsertAtTail(T value)
        {
                InsertAt(this.Tail, value);
        }

        public void InsertAt(ILink cur, T value)
        {
            if (InsertWasFirst(value))
                return;

            if (comparer.Compare(value, (T)cur) < 0)
                insertLesser(cur, value);
            else
                insertGreater(cur, value);
        }

        public override void InsertAfter(ILink cur, T value)
        {
            InsertAt(cur, value);
        }

        public override void InsertBefore(ILink cur, T value)
        {
            InsertAt(cur, value);
        }

        #region privates

        void insertGreater(ILink cur, T value)
        {
            ILink root = this.Tail;

            int compare = 0;
            while ((compare = comparer.Compare(value, (T)cur)) >= 0 && cur.Next != root)
                cur = cur.Next;

            if (compare < 0)
                base.InsertBefore(cur, value);
            else
                base.InsertAfter(cur, value);
        }

        void insertLesser(ILink cur, T value)
        {
            ILink root = this.Head;

            int compare = 0;
            while ((compare = comparer.Compare(value, (T) cur)) < 0 && cur != root)
                cur = cur.Prev;

            if (compare < 0)
                base.InsertBefore(cur, value);
            else
                base.InsertAfter(cur, value);
        }

        #endregion
    }

}
