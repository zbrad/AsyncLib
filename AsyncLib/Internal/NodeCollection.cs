using System;
using C = System.Collections;
using G = System.Collections.Generic;
using System.Threading;

namespace ZBrad.AsyncLib
{
    internal abstract class NodeCollection<N> : INodeCollection<N> where N : INode, IEquatable<N>
    {
        public INode Root { get { return this.Head; } }

        internal long version;
        public long Version { get { return this.version; } }

        int count;
        public int Count { get { return count; } }

        public INode Head { get; protected set; }

        public INode Tail { get; protected set; }

        bool G.ICollection<N>.IsReadOnly { get { return false; } }


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
            var cur = this.Head;
            while (cur != null)
            {
                array[arrayIndex + i] = (N) cur;
                cur = (N) cur.Next;
                i++;
            }
        }

        public virtual void Clear()
        {
            var cur = this.Head;
            this.Head = this.Tail = default(N);
            ClearCount();

            while (cur != null)
            {
                var next = cur.Next;
                cur.Next = cur.Prev = null;
                cur = (N) next;
            }
        }

        //protected void Unlink(INode a)
        //{
        //    if (a.Next == a)
        //    {
        //        // removing last item
        //        this.Head = this.Tail = null;
        //    }
        //    else
        //    {
        //        // removing from inner link
        //        a.Next.Prev = a.Prev;
        //        a.Prev.Next = a.Next;

        //        if (Head == a)
        //            Head = a.Next;
        //        if (Tail == a)
        //            Tail = a.Prev;
        //    }

        //    // clear links and update count
        //    a.Prev = a.Next = null;
        //    DecrementCount();
        //}

        public G.IEnumerator<N> GetEnumerator()
        {
            return new NodesEnum<N>(this);
        }

        C.IEnumerator C.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        //public virtual void Remove(N node)
        //{
        //    if (node == null || this.Count == 0)
        //        return;

        //    if (node.Prev == null && node.Next == null)
        //        return;

        //    Unlink(node);
        //}

        //public virtual N RemoveFromHead()
        //{
        //    if (this.Count == 0)
        //        return null;

        //    var node = this.Head;
        //    Unlink(node);
        //    return (N) node;
        //}

        //public virtual N RemoveFromTail()
        //{
        //    if (this.Count == 0)
        //        return null;

        //    var node = this.Tail;
        //    Unlink(node);
        //    return (N) node;
        //}

        public abstract void Add(N item);

        public abstract bool Contains(N item);

        public abstract bool Remove(N item);
    }
}
