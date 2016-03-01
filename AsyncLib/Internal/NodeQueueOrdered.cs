using C = System.Collections;
using G = System.Collections.Generic;
using S = System;

namespace ZBrad.AsyncLib
{
    internal class NodeQueueOrdered<N> : IQueueOrdered<N> where N : INode, S.IEquatable<N>, S.IComparable<N>
    {
        NodeListOrdered<N> nodes = new NodeListOrdered<N>();

        public long Version { get { return nodes.Version; } }

        public INode Root { get { return nodes.Root; } }

        public int Count { get { return nodes.Count; } }

        bool G.ICollection<N>.IsReadOnly { get { return false; } }


        public virtual bool Enqueue(N node)
        {
            nodes.InsertFromTail(node);
            return true;
        }

        public virtual N Dequeue()
        {
            return nodes.RemoveFromHead();
        }

        public virtual N PeekHead()
        {
            if (nodes.Count == 0)
                return default(N);

            return (N) nodes.Head;
        }

        public virtual N PeekTail()
        {
            if (nodes.Count == 0)
                return default(N);

            return (N) nodes.Tail;
        }

        public virtual void CopyTo(N[] array, int arrayIndex)
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

        void G.ICollection<N>.Add(N item)
        {
            this.Enqueue(item);
        }

        void G.ICollection<N>.Clear()
        {
            nodes.Clear();
        }

        bool G.ICollection<N>.Contains(N item)
        {
            return nodes.Contains(item);
        }

        bool G.ICollection<N>.Remove(N item)
        {
            return nodes.Remove(item);
        }
    }
}
