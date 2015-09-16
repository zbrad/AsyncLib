using System;
using System.Threading;
using System.Threading.Tasks;
using C = System.Collections;
using G = System.Collections.Generic;
using U = System.Collections.Concurrent;

namespace ZBrad.AsyncLib
{
    internal class WaitQueue<N> : INodeQueueAsync<N>, IWaitable, IWaitQueue<N> where N : class, INode
    {
        AwaitLock locker = new AwaitLock();
        NodeQueue<N> queue = new NodeQueue<N>();
        bool isEnded = false;
        static Task<N> Cancelled;
        // we have to use list to allow cancel cleanup
        NodeList<Waiter<N>> waiters = new NodeList<Waiter<N>>();
        public int WaitCount { get { return waiters.Count; } }

        static WaitQueue()
        {
            var tcs = new TaskCompletionSource<N>();
            tcs.SetCanceled();
            Cancelled = tcs.Task;
        }

        internal Task<AwaitLock.Releaser> GetAwait()
        {
            return locker.WaitAsync();
        }

        public int Count { get { return queue.Count; } }

        int INodeCollectionAsync<N>.Version { get { return queue.Version; } }

        INode INodeCollectionAsync<N>.Root { get { return queue.Root; } }

        public async Task EndEnqueue()
        {
            using (await locker.WaitAsync())
            {
                isEnded = true;
            }
        }

        public async Task<bool> IsEnded()
        {
            using (await locker.WaitAsync())
            {
                return isEnded;
            }
        }

        public async Task<bool> IsComplete()
        {
            using (await locker.WaitAsync())
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

        public async Task<bool> IsEmpty()
        {
            using (await locker.WaitAsync())
            {
                return queue.Count == 0;
            }
        }

        public Task Enqueue(N item)
        {
            return enqueue(item, CancellationToken.None);
        }

        public Task Enqueue(N item, CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return Cancelled;
            return enqueue(item, token);
        }

        async Task enqueue(N item, CancellationToken token)
        {
            using (await locker.WaitAsync(token))
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

        public Task<N> Dequeue()
        {
            return dequeue(CancellationToken.None);
        }

        public Task<N> Dequeue(CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return Cancelled;

            return dequeue(token);
        }
            
        async Task<N> dequeue(CancellationToken token)
        {
            Waiter<N> waiter = null;

            using (await locker.WaitAsync(token))
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
                using (await locker.WaitAsync())
                {
                    waiters.Remove(w);
                }

                w.Completed(null);
            });
        }

        public N PeekHead()
        {
            return queue.PeekHead();
        }

        public N PeekTail()
        {
            return queue.PeekTail();
        }

        public Task CopyTo(N[] array, int arrayIndex)
        {
            return copyTo(array, arrayIndex, CancellationToken.None);
        }

        public Task CopyTo(N[] array, int arrayIndex, CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return Cancelled;

            return copyTo(array, arrayIndex, token);
        }

        async Task copyTo(N[] array, int arrayIndex, CancellationToken token)
        { 
            int i = 0;
            var iter = this.GetAsyncEnumerator();

            while (await iter.MoveNext())
            {
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();

                array[arrayIndex + i] = iter.Current;
                i++;
            }
        }

        public INodeEnumeratorAsync<N> GetAsyncEnumerator()
        {
            return new NodesEnumAsync<N>(this);
        }
    }

    //public class PublicQueue<T>: IComparable<T> where T : IEquatable<T>
    //{
    //    internal ValueQueue<T> queue = null;
    //    //internal ItemQueueObserve<T> observer = null;
    //    AwaitLock locker = new AwaitLock();

    //    WaiterQueue waiters = new WaiterQueue();

    //    public Queue()
    //    {
    //        queue = new ValueQueue<T>();
    //        //observer = new QueueObserve<T>(queue);
    //    }

    //    public int Count { get { return queue.Count; } }

    //    //public async Task<T> PeekHead()
    //    //{
    //    //    using (await locker.WaitAsync())
    //    //    {
    //    //        return observer.PeekHead();
    //    //    }
    //    //}

    //    //public async Task<T> PeekTail()
    //    //{
    //    //    using (await locker.WaitAsync())
    //    //    {
    //    //        return observer.PeekTail();
    //    //    }
    //    //}

    //    public async Task<T> DequeueAsync(CancellationToken token)
    //    {
    //        using (await locker.WaitAsync(token))
    //        {
    //            if (queue.IsEmpty)
    //            {
    //                if (queue.IsInsertComplete)
    //                    return default(T);

    //                Waiter<T> a = new Waiter<T>();
    //                return await a;
    //            }

    //            Value<T> node = this.queue.RemoveFromHead();

    //            // if we have emptied it and it is complete, remove all other waiters
    //            if (queue.IsInsertComplete)
    //                ClearWaiters();

    //            if (node == null)
    //                return default(T);

    //            return node.Item;
    //        }
    //    }

    //    public async Task<T> DequeueAsync()
    //    {
    //        return await this.DequeueAsync(CancellationToken.None);
    //    }

    //    public async Task<bool> IsEnded()
    //    {
    //        using (await locker.WaitAsync())
    //        {
    //            return queue.IsEnded;
    //        }
    //    }

    //    public void EnqueueComplete()
    //    {
    //        Task.Run(async () =>
    //        {
    //            using (await locker.WaitAsync())
    //            {
    //                if (queue.IsInsertComplete)
    //                    return;

    //                // if there are some entries left, allow them to be removed in order
    //                if (queue.Count > 0)
    //                    return;

    //                // else complete all waiters (there won't be anymore entries now)
    //                ClearWaiters();
    //            }
    //        }).Wait();
    //    }

    //    void ClearWaiters()
    //    {
    //        Waiter waiter;
    //        while ((waiter = waiters.RemoveFromHead()) != null)
    //            waiter.Completed();
    //    }

    //    public async Task EnqueueAsync(T t, CancellationToken token)
    //    {
    //        using (await locker.WaitAsync(token))
    //        {
    //            // cannot add if we've been marked complete for adding
    //            if (queue.IsInsertComplete)
    //                return;

    //            if (queue.IsEmpty)
    //            {
    //                var waiter = this.waiters.RemoveFromHead();
    //                if (waiter != null)
    //                {
    //                    waiter.Completed();
    //                    return;
    //                }
    //            }

    //            Add(t);
    //        }
    //    }

    //    public async Task<bool> TryEnqueueAsync(Func<IQueue<IValue<T>>, T> func)
    //    {
    //        return TryEnqueueAsync((o) => Task.FromResult<T>(func(o)), CancellationToken.None);
    //    }
    //    public async Task<bool> TryEnqueueAsync(Func<IQueue<IValue<T>>, T> func, CancellationToken token)
    //    {
    //        return TryEnqueueAsync((o) => Task.FromResult<T>(func(o)), token);
    //    }

    //    public async Task<bool> TryEnqueueAsync(Func<IQueue<IValue<T>>, Task<T>> func, CancellationToken token)
    //    {
    //        using (await locker.WaitAsync(token))
    //        {
    //            if (queue.IsInsertComplete)
    //                return false;

    //            T value = await func(queue);
    //            if (value == null)
    //                return false;

    //            Add(value);
    //            observer.IsEmpty = false;
    //            return true;
    //        }
    //    }

    //    public async Task<bool> TryEnqueueAsync(Func<IQueueObserve<T>, Task<T>> func)
    //    {
    //        return this.TryEnqueueAsync(func, CancellationToken.None);
    //    }

    //    internal virtual void Add(T t)
    //    {
    //        var node = new Value<T>(t);
    //        this.queue.InsertAtTail(node);
    //    }

    //    public async Task EnqueueAsync(T t)
    //    {
    //        await this.EnqueueAsync(t, CancellationToken.None);
    //    }

    //    // this should not be called for non-ordered wait queue
    //    int IComparable<T>.CompareTo(T other)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
