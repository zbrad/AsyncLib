using S = System;

using C = System.Collections;
using G = System.Collections.Generic;
using U = System.Collections.Concurrent;

using System.Threading;
using System.Threading.Tasks;

namespace ZBrad.AsyncLib
{
    public class WaitQueue<N> : IQueueAsync<N>, IWaitable where N : INode, S.IEquatable<N>
    {
        NodeQueue<N> queue = new NodeQueue<N>();
        bool isEnded = false;
        static Task<N> Cancelled { get { return (Task<N>)TaskEx.Cancelled; } }

        // we have to use list to allow cancel cleanup
        NodeQueue<Waiter<N>> waiters = new NodeQueue<Waiter<N>>();

        public int WaitCount { get { return waiters.Count; } }

        AwaitLock qlock = new AwaitLock();
        public AwaitLock Locker { get { return this.qlock; } }

        public int Count { get { return queue.Count; } }


        public long Version { get { return queue.Version; } }

        public INode Root { get { return queue.Root; } }

        bool G.ICollection<N>.IsReadOnly { get { return false; } }

        public void EndEnqueue()
        {
            endEnqueue().Wait();
        }

        async Task endEnqueue()
        {
            using (await this.Locker.WaitAsync())
            {
                isEnded = true;
            }
        }

        public Task<bool> IsEndedAsync()
        {
            return this.IsEndedAsync(CancellationToken.None);
        }

        public async Task<bool> IsEndedAsync(CancellationToken token)
        {
            using (await this.Locker.WaitAsync(token))
            {
                return isEnded;
            }
        }

        public async Task<bool> IsCompleteAsync(CancellationToken token)
        {
            using (await this.Locker.WaitAsync(token))
            {
                return isComplete();
            }
        }

        public Task<bool> IsCompleteAsync()
        {
            return this.IsCompleteAsync(CancellationToken.None);
        }

        bool isComplete()
        {
            if (isEnded && queue.Count == 0)
                return true;
            return false;
        }

        public async Task<bool> IsEmptyAsync(CancellationToken token)
        {
            using (await this.Locker.WaitAsync(token))
            {
                return queue.Count == 0;
            }
        }

        public Task<bool> IsEmptyAsync()
        {
            return this.IsEmptyAsync(CancellationToken.None);
        }

        public Task<bool> EnqueueAsync(N item)
        {
            return enqueue(item, CancellationToken.None);
        }

        public Task<bool> EnqueueAsync(N item, CancellationToken token)
        {
            return enqueue(item, token);
        }

        async Task<bool> enqueue(N item, CancellationToken token)
        {
            using (await this.Locker.WaitAsync(token))
            {
                // are we done adding?
                if (isEnded)
                    return false;
                queue.Enqueue(item);

                while (waiters.Count > 0 && queue.Count > 0)
                {
                    var w = waiters.Dequeue();
                    var x = queue.Dequeue();
                    w.Completed(x);
                }
            }

            return true;
        }

        public Task<N> DequeueAsync()
        {
            return dequeue(CancellationToken.None);
        }

        public Task<N> DequeueAsync(CancellationToken token)
        {
            return dequeue(token);
        }

        async Task<N> dequeue(CancellationToken token)
        {
            Waiter<N> waiter = null;

            using (await this.Locker.WaitAsync(token))
            {
                if (isComplete())
                    return default(N);

                if (waiters.Count == 0 && queue.Count == 1)
                    return queue.Dequeue();

                waiter = new Waiter<N>(token);
                if (token != CancellationToken.None)
                    waiter.OnCancel += Waiter_OnCancel;
                waiters.Enqueue(waiter);
            }

            N item = await waiter;
            return item;
        }

        private void Waiter_OnCancel(Waiter<N> w)
        {
            Task.Run(async () =>
            {
                using (await this.Locker.WaitAsync())
                {
                    // this is safe while we hold the lock
                    waiters.Remove(w);
                }

                w.Completed(default(N));
            });
        }


        public Task<N> PeekHeadAsync()
        {
            return PeekHeadAsync(CancellationToken.None);
        }

        public async Task<N> PeekHeadAsync(CancellationToken token)
        {
            using (await this.Locker.WaitAsync(token))
            {
                return queue.PeekHead();
            }
        }

        public Task<N> PeekTailAsync()
        {
            return PeekTailAsync(CancellationToken.None);
        }

        public async Task<N> PeekTailAsync(CancellationToken token)
        {
            using (await this.Locker.WaitAsync(token))
            {
                return queue.PeekTail();
            }
        }

        public Task<bool> CopyToAsync(N[] array, int arrayIndex)
        {
            return CopyToAsync(array, arrayIndex, CancellationToken.None);
        }

        public async Task<bool> CopyToAsync(N[] array, int arrayIndex, CancellationToken token)
        {
            using (await this.Locker.WaitAsync(token))
            {
                queue.CopyTo(array, arrayIndex);
                return true;
            }
        }

        public IAsyncEnumerator<N> GetAsyncEnumerator()
        {
            return new NodesEnumAsync<N>(this);
        }

        public async Task<bool> ContainsAsync(N item, CancellationToken token)
        {
            using (await this.Locker.WaitAsync(token))
            {
                return queue.Contains(item);
            }
        }

        public Task<bool> ClearAsync()
        {
            return ClearAsync(CancellationToken.None);
        }

        public async Task<bool> ClearAsync(CancellationToken token)
        {
            using (await this.Locker.WaitAsync())
            {
                queue.Clear();
                return true;
            }
        }

        public Task<bool> ContainsAsync(N item)
        {
            return ContainsAsync(item, CancellationToken.None);
        }

        public Task<bool> RemoveAsync(N item)
        {
            return RemoveAsync(item, CancellationToken.None);
        }

        public async Task<bool> RemoveAsync(N item, CancellationToken token)
        {
            using (await this.Locker.WaitAsync(token))
            {
                return queue.Remove(item);
            }
        }

        public Task<bool> AddAsync(N item)
        {
            return AddAsync(item, CancellationToken.None);
        }

        public Task<bool> AddAsync(N item, CancellationToken token)
        {
            return EnqueueAsync(item, token);
        }

        G.IEnumerator<N> G.IEnumerable<N>.GetEnumerator() { throw new S.NotImplementedException("use GetAsync"); }
        void G.ICollection<N>.Add(N item) { throw new S.NotImplementedException("use AddAsync"); }
        void G.ICollection<N>.Clear() { throw new S.NotImplementedException("use ClearAsync"); }
        bool G.ICollection<N>.Contains(N item) { throw new S.NotImplementedException("use ContainsAsync"); }
        void G.ICollection<N>.CopyTo(N[] array, int arrayIndex) { throw new S.NotImplementedException("use CopyToAsync"); }
        bool G.ICollection<N>.Remove(N item) { throw new S.NotImplementedException("use RemoveAsync"); }
        C.IEnumerator C.IEnumerable.GetEnumerator() { throw new S.NotImplementedException("use GetAsyncEnumerator"); }

    }
}
