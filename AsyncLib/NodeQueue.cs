using System;
using C = System.Collections;
using G = System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ZBrad.AsyncLib
{
    internal class NodeQueue<N> : INodeQueue<N> where N : class, INode
    {
        NodeList<N> nodes = new NodeList<N>();

        public int Version { get { return nodes.Version; } }

        public INode Root { get { return nodes.Root; } }

        public int Count { get { return nodes.Count; } }

        public virtual void Enqueue(N node)
        {
            nodes.InsertAtTail(node);
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
    }
}

