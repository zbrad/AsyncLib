using C = System.Collections;
using G = System.Collections.Generic;
using System;

namespace ZBrad.AsyncLib
{
    internal class NodeQueueOrdered<N> : INodeQueueOrdered<N> where N : class, INodeComparable<N>
    {
        NodeListOrdered<N> nodes = new NodeListOrdered<N>();

        public int Version { get { return nodes.Version; } }

        public INode Root { get { return nodes.Root; } }

        public int Count { get { return nodes.Count; } }

        public virtual void Enqueue(N node)
        {
            nodes.InsertFromTail(node);
        }

        public virtual N Dequeue()
        {
            return nodes.RemoveFromHead();
        }

        public virtual N PeekHead()
        {
            if (nodes.Count == 0)
                return null;

            return (N) nodes.Head;
        }

        public virtual N PeekTail()
        {
            if (nodes.Count == 0)
                return null;

            return (N) nodes.Tail;
        }

        void INodeCollection<N>.CopyTo(N[] array, int arrayIndex)
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
    }

}
