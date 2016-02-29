using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZBrad.AsyncLib.Public
{
    public class Queue<N> : IQueue<N> where N : class, INode
    {
        WaitQueue<N> queue = new WaitQueue<N>();

        public int Count { get { return queue.CountAsync.Result; } }

        public INode Root { get { return queue.GetRootAsync(queue.Version).Result; } }

        public int Version { get { return queue.Version; } }

        public void CopyTo(N[] array, int arrayIndex)
        {
            queue.CopyToAsync(array, arrayIndex).Wait();
        }

        public N Dequeue()
        {
            return queue.DequeueAsync().Result;
        }

        public void Enqueue(N item)
        {
            queue.EnqueueAsync(item).Wait();
        }

        public IEnumerator<N> GetEnumerator()
        {
            return new SynchronousEnumerator<N>(queue);
        }

        public N PeekHead()
        {
            return queue.PeekHeadAsync().Result;
        }

        public N PeekTail()
        {
            return queue.PeekTailAsync().Result;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }


    public class SynchronousEnumerator<N> : IEnumerator<N> where N : class, INode
    {
        INodeEnumeratorAsync<N> asyncEnum;

        public SynchronousEnumerator(INodeEnumerableAsync<N> collection)
        {
            this.asyncEnum = collection.GetEnumeratorAsync();
        }

        public N Current { get { return this.asyncEnum.Current; } }

        object IEnumerator.Current { get { return this.Current; } }

        public void Dispose()
        {
            // nothing to do
        }

        public bool MoveNext()
        {
            return this.asyncEnum.MoveNextAsync().Result;
        }

        public void Reset()
        {
            this.asyncEnum.ResetAsync().Wait();
        }
    }
}
