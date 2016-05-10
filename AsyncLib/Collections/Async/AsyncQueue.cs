using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections;

namespace ZBrad.AsyncLib.Collections
{
    internal class AsyncQueue<T> : IAsyncQueue<T>
    {
        long version = 0;

        public AsyncQueue()
        {
            this.Queue = new QueueW();
        }

        public int Count { get { return this.Queue.Count; } }

        public AwaitLock Lock { get; private set; } = new AwaitLock();

        public IQueue<T> Queue { get; private set; }

        public async Task CopyTo(T[] array, int arrayIndex, CancellationToken token)
        {
            using (await this.Lock.Wait(token))
            {
                this.Queue.CopyTo(array, arrayIndex);
            }
        }

        public async Task<T> Dequeue(CancellationToken token)
        {
            using (await this.Lock.Wait(token))
            {
                version++;
                return this.Queue.Dequeue();
            }
        }

        public async Task Enqueue(T item, CancellationToken token)
        {
            using (await this.Lock.Wait(token))
            {
                version++;
                this.Queue.Enqueue(item);
            }
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator()
        {
            return new Enumerator(this);
        }

        public async Task<T> Peek(CancellationToken token)
        {
            using (await this.Lock.Wait(token))
            {
                return this.Queue.Peek();
            }
        }

        public async Task TrimExcess(CancellationToken token)
        {
            using (await this.Lock.Wait(token))
            {
                version++;
                this.Queue.TrimExcess();
            }
        }

        class Enumerator : IAsyncEnumerator<T>
        {
            AsyncQueue<T> queue;
            IEnumerator<T> e;
            long version;

            public Enumerator(AsyncQueue<T> q)
            {
                this.queue = q;
            }

            public T Current { get { return e.Current; } }

            public void Dispose()
            {
                if (e == null)
                    e.Dispose();

                e = null;
                queue = null;
            }

            public async Task<bool> MoveNext(CancellationToken token)
            {
                using (await queue.Lock.Wait(token))
                {
                    if (e == null)
                    {
                        e = queue.Queue.GetEnumerator();
                        version = queue.version;
                    }

                    if (version != queue.version)
                        throw new InvalidOperationException("queue modified");

                    return e.MoveNext();
                }
            }

            public Task Reset(CancellationToken token)
            {
                e = null;
                return TaskEx.True;
            }
        }

        // a wrapper for generic queue that implements IQueue interface
        class QueueW : IQueue<T>
        {
            Queue<T> queue;

            public QueueW()
            {
                this.queue = new Queue<T>();
            }

            public int Count { get { return queue.Count; } }

            public void Clear()
            {
                queue.Clear();
            }

            public bool Contains(T item)
            {
                return queue.Contains(item);
            }

            public void CopyTo(T[] array, int index)
            {
                queue.CopyTo(array, index);
            }

            public T Dequeue()
            {
                return queue.Dequeue();
            }

            public void Enqueue(T item)
            {
                queue.Enqueue(item);
            }

            public IEnumerator<T> GetEnumerator()
            {
                return queue.GetEnumerator();
            }

            public T Peek()
            {
                return queue.Peek();
            }

            public T[] ToArray()
            {
                return queue.ToArray();
            }

            public void TrimExcess()
            {
                queue.TrimExcess();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }
    }
}
