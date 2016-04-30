using System;

using C = System.Collections;
using G = System.Collections.Generic;
using U = System.Collections.Concurrent;

using System.Threading;
using System.Threading.Tasks;
using ZBrad.AsyncLib.Collections;
using ZBrad.AsyncLib.Nodes;

namespace ZBrad.AsyncLib
{
    public class WaitQueue<N> : IValueAsyncCollection<N>, IWaitable where N : IEquatable<N>
    {
        static Task<N> faulted;

        static WaitQueue()
        {
            var tcs = new TaskCompletionSource<N>();
            tcs.SetException(new TaskCanceledException());
            faulted = tcs.Task;
        }

        Queue<N> queue = new Queue<N>();
        bool isEnded = false;

        // we have to use list to allow cancel cleanup
        LinkedList<Waiter<N>> waiters = new LinkedList<Waiter<N>>();

        public int WaitCount { get { return waiters.Count; } }

        AwaitLock qlock = new AwaitLock();
        public AwaitLock Locker { get { return this.qlock; } }

        public int Count { get { return queue.Count; } }

        public long Version { get { return queue.Version; } }

        public bool IsReadOnly { get { return false; } }

        IValue<N> IValueAsyncCollection<N>.Root { get { return (IValue<N>) queue.Root; } }

        public Task EndEnqueueAsync()
        {
            return EndEnqueueAsync(CancellationToken.None);
        }

        public async Task EndEnqueueAsync(CancellationToken token)
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
            if (token.IsCancellationRequested)
                return TaskEx.FaultedBool;

            return enqueue(item, token);
        }

        async Task<bool> enqueue(N item, CancellationToken token)
        {
            using (await this.Locker.WaitAsync(token))
            {
                // are we done adding?
                if (isEnded)
                    return false;

                // special case for havings waiters and no queue elements
                if (waiters.Count > 0 && queue.Count == 0)
                {
                    var w = waiters.RemoveFromHead();
                    w.Completed(item);
                    return true;
                }

                queue.Enqueue(item);
                completeWaiters();
            }

            return true;
        }

        void completeWaiters()
        {
            while (waiters.Count > 0 && queue.Count > 0)
            {
                var w = waiters.RemoveFromHead();
                var x = queue.Dequeue();
                w.Completed(x);
            }
        }

        public Task<N> DequeueAsync()
        {
            return dequeue(CancellationToken.None);
        }

        public Task<N> DequeueAsync(CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return faulted;

            return dequeue(token);
        }

        async Task<N> dequeue(CancellationToken token)
        {
            Waiter<N> waiter = null;

            using (await this.Locker.WaitAsync(token))
            {
                if (isComplete())
                    return default(N);

                if (waiters.Count == 0 && queue.Count > 0)
                    return queue.Dequeue();

                waiter = new Waiter<N>(token);
                if (token != CancellationToken.None)
                    waiter.OnCancel += Waiter_OnCancel;
                waiters.InsertAtTail(waiter);

                completeWaiters();
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

        public Task CopyToAsync(N[] array, int arrayIndex)
        {
            return CopyToAsync(array, arrayIndex, CancellationToken.None);
        }

        public async Task CopyToAsync(N[] array, int arrayIndex, CancellationToken token)
        {
            using (await this.Locker.WaitAsync(token))
            {
                queue.CopyTo(array, arrayIndex);
            }
        }

        public async Task<bool> ContainsAsync(N item, CancellationToken token)
        {
            using (await this.Locker.WaitAsync(token))
            {
                return queue.Contains(item);
            }
        }

        public Task ClearAsync()
        {
            return ClearAsync(CancellationToken.None);
        }

        public async Task ClearAsync(CancellationToken token)
        {
            using (await this.Locker.WaitAsync())
            {
                queue.Clear();
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

        public IValueAsyncEnumerator<N> GetAsyncEnumerator()
        {
            return new NodeEnumAsync<N>(this);
        }
    }
}
