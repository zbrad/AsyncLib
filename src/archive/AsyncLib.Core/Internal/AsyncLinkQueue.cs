using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZBrad.AsyncLib.Collections
{
    internal class AsyncLinkQueue<T> : IAsyncLinkQueue<T> where T : ILink, IEquatable<T>
    {
        public AwaitLock Lock { get; private set; } = new AwaitLock();

        public ILinkQueue<T> Queue { get; private set; }

        public int Count { get { return this.Queue.Count; } }

        public AsyncLinkQueue()
        {
            this.Queue = new LinkQueue<T>(); 
        }

        public async Task<T> Peek(CancellationToken token)
        {
            using (await this.Lock.Wait(token))
            {
                return this.Queue.Dequeue();
            }
        }

        public async Task<T> Dequeue(CancellationToken token)
        {
            using (await this.Lock.Wait(token))
            {
                return this.Queue.Dequeue();
            }
        }

        public async Task Enqueue(T item, CancellationToken token)
        {
            using (await this.Lock.Wait(token))
            {
                this.Queue.Enqueue(item);
            }
        }

        public async Task TrimExcess(CancellationToken token)
        {
            using (await this.Lock.Wait(token))
            {
                this.Queue.TrimExcess();
            }
        }

        public async Task Clear(CancellationToken token)
        {
            using (await this.Lock.Wait(token))
            {
                this.Queue.Clear();
            }
        }

        public async Task<bool> Contains(T item, CancellationToken token)
        {
            using (await this.Lock.Wait(token))
            {
                return this.Queue.Contains(item);
            }
        }

        public async Task CopyTo(T[] array, int arrayIndex, CancellationToken token)
        {
            using (await this.Lock.Wait(token))
            {
                this.Queue.CopyTo(array, arrayIndex);
            }
        }

        public async Task<ITry<T>> TryFromHead(T value, CancellationToken token)
        {
            using (await this.Lock.Wait(token))
            {
                return this.Queue.TryHead(value);
            }
        }

        public async Task<ITry<T>> TryFromTail(T value, CancellationToken token)
        {
            using (await this.Lock.Wait(token))
            {
                return this.Queue.TryTail(value);
            }
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator()
        {
            return new QueueLinksEnumerator(this);
        }


        class QueueLinksEnumerator : IAsyncEnumerator<T>
        {
            AsyncLinkQueue<T> queue;
            IEnumerator<T> e;

            public QueueLinksEnumerator(AsyncLinkQueue<T> queue)
            {
                this.queue = queue;
            }

            public T Current { get { return e.Current; } }

            public void Dispose()
            {
                if (e != null)
                    e.Dispose();

                e = null;
                queue = null;
            }

            public async Task<bool> MoveNext(CancellationToken token)
            {
                using (await queue.Lock.Wait(token))
                {
                    if (e == null)
                        e = queue.Queue.GetEnumerator();

                    return e.MoveNext();
                }
            }

            public Task Reset(CancellationToken token)
            {
                e = null;
                return TaskEx.True;
            }
        }
    }
}
