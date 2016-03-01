using C = System.Collections;
using G = System.Collections.Generic;
using S = System;

namespace ZBrad.AsyncLib
{
    internal class NodeListOrdered<N> : INodeListOrdered<N> where N : INode, S.IEquatable<N>, S.IComparable<N>
    {
        NodeList<N> nodes = new NodeList<N>();

        public int Count { get { return nodes.Count; } }
        public INode Root { get { return nodes.Head; } }
        public INode Head { get { return nodes.Head; } }
        public INode Tail { get { return nodes.Tail; } }
        public long Version { get { return nodes.Version; } }

        bool G.ICollection<N>.IsReadOnly { get { return false; } }


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

        public bool InsertFrom(N position, N newnode)
        {
            if (position == null || newnode == null)
                return false;

            if (newnode.Prev != null || newnode.Next != null)
                return false;

            if (nodes.Count == 0)
            {
                nodes.InsertAtHead(newnode);
                return true;
            }

            if (newnode.CompareTo(position) >= 0)
                insertGreater(position, newnode);
            else
                insertLesser(position, newnode);

            return true;
        }

        public bool InsertFromHead(N newnode)
        {
            if (newnode == null)
                return false;

            if (newnode.Prev != null || newnode.Next != null)
                return false;

            if (nodes.Count == 0)
            {
                nodes.InsertAtHead(newnode);
                return true;
            }

            return InsertFrom((N) this.Head, newnode);
        }

        public bool InsertFromTail(N newnode)
        {
            if (newnode == null)
                return false;

            if (newnode.Prev != null || newnode.Next != null)
                return false;

            if (nodes.Count == 0)
            {
                nodes.InsertAtHead(newnode);
                return false;
            }

            return InsertFrom((N) this.Tail, newnode);
        }

        public bool Remove(N node)
        {
            if (node == null)
                return false;

            return nodes.Remove(node);
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

        public void Add(N item)
        {
            this.InsertFromTail(item);
        }

        public void Clear()
        {
            nodes.Clear();
        }

        public bool Contains(N item)
        {
            return nodes.Contains(item);
        }
    }

}
