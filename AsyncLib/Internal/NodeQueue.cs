using System;
using C = System.Collections;
using G = System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ZBrad.AsyncLib
{
    internal class NodeQueue<N> : IQueue<N> where N : INode, IEquatable<N>
    {
        NodeList<N> nodes = new NodeList<N>();

        public long Version { get { return nodes.Version; } }

        public INode Root { get { return nodes.Root; } }

        public int Count { get { return nodes.Count; } }

        public bool IsReadOnly { get { return false; } }

        bool G.ICollection<N>.IsReadOnly { get { return false; } }

        public virtual bool Enqueue(N node)
        {
            return nodes.InsertAtTail(node);
        }

        public virtual N Dequeue()
        {
            return nodes.RemoveFromHead();
        }

        public virtual N PeekHead()
        {
            return (N) nodes.Head;
        }

        public virtual N PeekTail()
        {
            return (N) nodes.Tail;
        }

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

        public void Add(N item)
        {
            this.Enqueue(item);
        }

        public void Clear()
        {
            nodes.Clear();
        }

        public bool Contains(N item)
        {
            return nodes.Contains(item);
        }

        public bool Remove(N item)
        {
            return nodes.Remove(item);
        }
    }
}

