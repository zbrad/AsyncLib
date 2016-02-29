using System;
using System.Threading;
using System.Threading.Tasks;
using C = System.Collections;
using G = System.Collections.Generic;
using U = System.Collections.Concurrent;

namespace ZBrad.AsyncLib
{
    public class WaitQueue<N> : IQueueAsync<N>, IWaitable where N : class, INode
    {
        NodeQueue<N> queue = new NodeQueue<N>();
        bool isEnded = false;
        static Task<N> Cancelled { get { return (Task<N>) TaskEx.Cancelled; } }

        // we have to use list to allow cancel cleanup
        NodeList<Waiter<N>> waiters = new NodeList<Waiter<N>>();

        public int WaitCount { get { return waiters.Count; } }

        AwaitLock qlock = new AwaitLock();
        public AwaitLock Locker { get { return this.qlock; } }

        public Task<int> CountAsync
        {
            get
            {
                return Task.Run<int>(async () =>
                {
                    using (await this.Locker.WaitAsync())
                    {
                        return queue.Count;
                    }
                });
            }
        }

        public int Version { get { return queue.Version; } }

        public async Task<INode> GetRootAsync(int version)
        {
            using (await this.Locker.WaitAsync())
            {
                if (version != this.Version)
                    throw new InvalidOperationException("Collection has been modified");

                return queue.Root;
            }
        }

        public void EndEnqueue()
        {
            Task.Run(async () =>
            {
                using (await this.Locker.WaitAsync())
                {
                    isEnded = true;
                }
            }).Wait();
        }

        public async Task<bool> IsEndedAsync()
        {
            using (await this.Locker.WaitAsync())
            {
                return isEnded;
            }
        }

        public async Task<bool> IsCompleteAsync()
        {
            using (await this.Locker.WaitAsync())
            {
                return isComplete();
            }
        }

        bool isComplete()
        {
            if (isEnded && queue.Count == 0)
                return true;
            return false;
        }

        public async Task<bool> IsEmptyAsync()
        {
            using (await this.Locker.WaitAsync())
            {
                return queue.Count == 0;
            }
        }

        public Task EnqueueAsync(N item)
        {
            return enqueue(item, CancellationToken.None);
        }

        public Task EnqueueAsync(N item, CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return Cancelled;
            return enqueue(item, token);
        }

        async Task enqueue(N item, CancellationToken token)
        {
            using (await this.Locker.WaitAsync(token))
            {
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();

                // are we done adding?
                if (isEnded)
                    return;

                if (queue.Count == 0 && waiters.Count > 0)
                {
                    // if only item, and already waiters, then just give first waiter this item
                    var w = waiters.RemoveFromHead();
                    w.Completed(item);
                    return;
                }

                queue.Enqueue(item);

                if (waiters.Count > 0)
                {
                    // get first waiter and first item
                    var w = waiters.RemoveFromHead();
                    var first = queue.Dequeue();
                    w.Completed(first);
                }
            }
        }

        public Task<N> DequeueAsync()
        {
            return dequeue(CancellationToken.None);
        }

        public Task<N> DequeueAsync(CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return Cancelled;

            return dequeue(token);
        }
            
        async Task<N> dequeue(CancellationToken token)
        {
            Waiter<N> waiter = null;

            using (await this.Locker.WaitAsync(token))
            {
                if (isComplete())
                    return null;

                if (queue.Count > 0)
                    return queue.Dequeue();

                waiter = new Waiter<N>(token);
                if (token != CancellationToken.None)
                    waiter.OnCancel += Waiter_OnCancel;

                waiters.InsertAtTail(waiter);
            }

            N item = await waiter;
            if (token.IsCancellationRequested)
                token.ThrowIfCancellationRequested();

            return item;
        }

        private void Waiter_OnCancel(Waiter<N> w)
        {
            Task.Run(async () =>
            {
                using (await this.Locker.WaitAsync())
                {
                    waiters.Remove(w);
                }

                w.Completed(null);
            });
        }

        public async Task<N> PeekHeadAsync()
        {
            using (await this.Locker.WaitAsync())
            {
                return queue.PeekHead();
            }
        }

        public async Task<N> PeekTailAsync()
        {
            using (await this.Locker.WaitAsync())
            {
                return queue.PeekTail();
            }
        }

        public Task CopyToAsync(N[] array, int arrayIndex)
        {
            return copyTo(array, arrayIndex, CancellationToken.None);
        }

        public Task CopyToAsync(N[] array, int arrayIndex, CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return Cancelled;

            return copyTo(array, arrayIndex, token);
        }

        async Task copyTo(N[] array, int arrayIndex, CancellationToken token)
        { 
            int i = 0;
            var iter = this.GetEnumeratorAsync();

            while (await iter.MoveNextAsync())
            {
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();

                array[arrayIndex + i] = iter.Current;
                i++;
            }
        }

        public INodeEnumeratorAsync<N> GetEnumeratorAsync()
        {
            return new NodesEnumAsync<N>(this);
        }
    }
}
