using C = System.Collections;
using G = System.Collections.Generic;
using System;

namespace ZBrad.AsyncLib
{
    internal class NodeListOrdered<N> : INodeListOrdered<N> where N : class, INodeComparable<N>
    {
        NodeList<N> nodes = new NodeList<N>();

        public int Count { get { return nodes.Count; } }
        public INode Root { get { return nodes.Head; } }

        public INode Head { get { return nodes.Head; } }

        public INode Tail { get { return nodes.Tail; } }

        public int Version { get { return nodes.Version; } }

        public void CopyTo(N[] array, int arrayIndex)
        {
            nodes.CopyTo(array, arrayIndex);
        }

        public G.IEnumerator<N> GetEnumerator()
        {
            return nodes.GetEnumerator();
        }

        C.IEnumerator C.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void InsertFrom(N position, N newnode)
        {
            if (position == null || newnode == null)
                return;

            if (newnode.Prev != null || newnode.Next != null)
                return;

            if (nodes.Count == 0)
            {
                nodes.InsertAtHead(newnode);
                return;
            }

            if (newnode.CompareTo(position) >= 0)
                insertGreater(position, newnode);
            else
                insertLesser(position, newnode);
        }

        public void InsertFromHead(N newnode)
        {
            if (newnode == null)
                return;

            if (newnode.Prev != null || newnode.Next != null)
                return;

            if (nodes.Count == 0)
            {
                nodes.InsertAtHead(newnode);
                return;
            }

            InsertFrom((N)this.Head, newnode);
        }

        public void InsertFromTail(N newnode)
        {
            if (newnode == null)
                return;

            if (newnode.Prev != null || newnode.Next != null)
                return;

            if (nodes.Count == 0)
            {
                nodes.InsertAtHead(newnode);
                return;
            }

            InsertFrom((N)this.Tail, newnode);
        }

        public void Remove(N node)
        {
            if (node == null)
                return;

            nodes.Remove(node);
        }

        public N RemoveFromHead()
        {
            return nodes.RemoveFromHead();
        }

        public N RemoveFromTail()
        {
            return nodes.RemoveFromTail();
        }

        #region privates

        void insertGreater(N cur, N node)
        {
            while (cur.Next != Head && node.CompareTo(cur) >= 0)
                cur = (N) cur.Next;
            nodes.InsertAfter(cur, node);
        }

        void insertLesser(N cur, N node)
        {
            while (cur.Prev != Tail && node.CompareTo(cur) < 0)
                cur = (N) cur.Prev;
            nodes.InsertBefore(cur, node);
        }

        #endregion
    }

}
