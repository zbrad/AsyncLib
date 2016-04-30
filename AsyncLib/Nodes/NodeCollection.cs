using System;
using C = System.Collections;
using G = System.Collections.Generic;
using System.Threading;
using ZBrad.AsyncLib.Links;

namespace ZBrad.AsyncLib.Nodes
{
    internal abstract class NodeCollection<N> : INodeCollection<N> where N : ILink
    {
        public abstract N Root { get; protected set; }

        long version;
        public long Version { get { return this.version; } }

        int count;
        public virtual int Count { get { return count; } }

        public bool IsReadOnly { get { return false; } }

        protected void IncrementCount()
        {
            Interlocked.Increment(ref count);
            Interlocked.Increment(ref version);
        }

        protected void DecrementCount()
        {
            Interlocked.Decrement(ref count);
            Interlocked.Increment(ref version);
        }

        protected void ClearCount()
        {
            Interlocked.Exchange(ref count, 0);
        }

        public virtual void CopyTo(N[] array, int arrayIndex)
        {
            int i = 0;
            var cur = this.Root;
            while (cur != null)
            {
                array[arrayIndex + i] = cur;
                cur = (N) cur.Next;
                i++;
            }
        }

        public virtual void Clear()
        {
            var cur = this.Root;
            ClearCount();

            while (cur != null)
            {
                var next = cur.Next;
                cur.Next = cur.Prev = null;
                cur = (N) next;
            }
        }

        public G.IEnumerator<N> GetEnumerator()
        {
            return new NodeEnum<N>(this);
        }

        C.IEnumerator C.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public abstract void Add(N item);

        public abstract bool Contains(N item);

        public abstract bool Remove(N item);
    }
}
