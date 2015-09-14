using System;
using System.Threading;
using System.Threading.Tasks;
using C = System.Collections;
using G = System.Collections.Generic;
using U = System.Collections.Concurrent;

namespace ZBrad.AsyncLib
{
    internal class WaitQueue<N> : INodeQueueAsync<N>, IWaitable where N : class, INode
    {
        AwaitLock locker = new AwaitLock();
        NodeQueue<N> queue = new NodeQueue<N>();

        bool isEnded = false;
        public bool IsEnded { get { return isEnded; } }

        public int Count { get { return queue.Count; } }
        
        int INodeCollectionAsync<N>.Version { get { return queue.Version; } }

        INode INodeCollectionAsync<N>.Root { get { return queue.Root; } }

        public void Completed()
        {
            isEnded = true;
        }

        public async Task<bool> IsCompleteAsync()
        {
            using (await locker.WaitAsync())
            {
                if (isEnded && queue.Count == 0)
                    return true;
                return false;
            }
        }

        public async Task<bool> IsEmptyAsync()
        {
            using (await locker.WaitAsync())
            {
                return queue.Count == 0;
            }
        }

        public async Task Enqueue(N item)
        {
            using (await locker.WaitAsync())
            {
                queue.Enqueue(item);
            }
        }

        public async Task Enqueue(N item, CancellationToken token)
        {
            using (await locker.WaitAsync(token))
            {
                queue.Enqueue(item);
            }
        }

        public async Task<N> Dequeue()
        {
            using (await locker.WaitAsync())
            {
                return queue.Dequeue();
            }
        }

        public async Task<N> Dequeue(CancellationToken token)
        {
            using (await locker.WaitAsync(token))
            {
                return queue.Dequeue();
            }
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
            return CopyTo(array, arrayIndex, CancellationToken.None);
        }

        public async Task CopyTo(N[] array, int arrayIndex, CancellationToken token)
        {
            int i = 0;
            var iter = this.GetAsyncEnumerator();

            while (await iter.MoveNext())
            {
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
