using System;
using C = System.Collections;
using G = System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZBrad.AsyncLib.Public
{
    public class Queue<N> : IQueue<N> where N : INode, IEquatable<N>
    {
        WaitQueue<N> queue = new WaitQueue<N>();

        public int Count { get { return queue.Count; } }

        public INode Root { get { return queue.Root; } }

        public long Version { get { return queue.Version; } }

        public void CopyTo(N[] array, int arrayIndex)
        {
            queue.CopyToAsync(array, arrayIndex).Wait();
        }

        bool G.ICollection<N>.IsReadOnly { get { return false; } }

        public N Dequeue()
        {
            return queue.DequeueAsync().Result;
        }

        public bool Enqueue(N item)
        {
            return queue.EnqueueAsync(item).Result;
        }

        public N PeekHead()
        {
            return queue.PeekHeadAsync().Result;
        }

        public N PeekTail()
        {
            return queue.PeekTailAsync().Result;
        }

        G.IEnumerator<N> G.IEnumerable<N>.GetEnumerator()
        {
            return new SynchronousEnumerator<N>(queue.GetAsyncEnumerator());
        }

        C.IEnumerator C.IEnumerable.GetEnumerator()
        {
            return ((G.IEnumerable<N>)this).GetEnumerator();
        }

        void G.ICollection<N>.Add(N item)
        {
            queue.AddAsync(item).Wait();
        }

        public void Clear()
        {
            queue.ClearAsync().Wait();
        }

        bool G.ICollection<N>.Contains(N item)
        {
            return queue.Contains(item);
        }

        bool G.ICollection<N>.Remove(N item)
        {
            return queue.RemoveAsync(item).Result;
        }

    }


    public class SynchronousEnumerator<N> : G.IEnumerator<N> where N : INode, IEquatable<N>
    {
        IAsyncEnumerator<N> asyncEnum;

        public SynchronousEnumerator(IAsyncEnumerator<N> asyncEnum)
        {
            this.asyncEnum = asyncEnum;
        }

        public N Current { get { return this.asyncEnum.Current; } }

        object C.IEnumerator.Current { get { return this.Current; } }

        public void Dispose()
        {
            if (asyncEnum != null)
                ((IDisposable)asyncEnum).Dispose();
            asyncEnum = null;
        }

        public bool MoveNext()
        {
            return this.asyncEnum.MoveNextAsync().Result;
        }

        public void Reset()
        {
            this.asyncEnum.Reset();
        }
    }
}
