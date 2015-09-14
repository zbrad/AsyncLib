using System.Threading;
using System.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ZBrad.AsyncLib
{
    //public class OrderedQueue<T> where T : IComparable<T>
    //{
    //    SimpleQueue<OrderedNode<T>> queue = null;
    //    OrderedQueueObserve<T> observer = null;
    //    UsingSemaphore locker = new UsingSemaphore();
    //    SimpleQueue<Waiter<OrderedNode<T>>> waiters = new SimpleQueue<Waiter<OrderedNode<T>>>();

    //    public OrderedQueue()
    //    {
    //        queue = new SimpleQueue<OrderedNode<T>>();
    //        observer = new OrderedQueueObserve<T>(queue);
    //    }

    //    public int Count { get { return observer.Count; } }

    //    public async Task<T> PeekHead()
    //    {
    //        using (await locker.WaitAsync())
    //        {
    //            return observer.PeekHead();
    //        }
    //    }

    //    public async Task<T> PeekTail()
    //    {
    //        using (await locker.WaitAsync())
    //        {
    //            return observer.PeekTail();
    //        }
    //    }

    //    public async Task<T> DequeueAsync(CancellationToken token)
    //    {
    //        using (await locker.WaitAsync(token))
    //        {
    //            if (observer.IsEmpty)
    //            {
    //                if (observer.IsEnqueueComplete)
    //                    return default(T);

    //                Waiter<T> a = new Waiter<T>();
    //                return await a;
    //            }

    //            OrderedNode<T> node = this.queue.RemoveFromHead();

    //            // did we empty it?
    //            if (queue.Count == 0)
    //            {
    //                observer.IsEmpty = true;

    //                // if we have emptied it and it is complete, remove all other waiters
    //                if (observer.IsEnqueueComplete)
    //                    ClearWaiters();
    //            }

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
    //            return observer.IsEnded;
    //        }
    //    }

    //    public void EnqueueComplete()
    //    {
    //        Task.Run(async () =>
    //        {
    //            using (await locker.WaitAsync())
    //            {
    //                if (observer.IsEnqueueComplete)
    //                    return;

    //                observer.IsEnqueueComplete = true;

    //                // if there are some entries left, allow them to be removed in order
    //                if (queue.Count > 0)
    //                    return;

    //                // else complete all waiters
    //                ClearWaiters();
    //            }
    //        }).Wait();
    //    }

    //    void ClearWaiters()
    //    {
    //        Waiter<OrderedNode<T>> waiter;
    //        while ((waiter = waiters.RemoveFromHead()) != null)
    //            waiter.Completed(null);
    //    }

    //    public async Task EnqueueAsync(T t, CancellationToken token)
    //    {
    //        using (await locker.WaitAsync(token))
    //        {
    //            // cannot add if we've been marked complete for adding
    //            if (observer.IsEnqueueComplete)
    //                return;

    //            if (observer.IsEmpty)
    //            {
    //                Waiter<OrderedNode<T>> a = this.waiters.RemoveFromHead();
    //                if (a != null)
    //                {
    //                    a.Completed(null);
    //                    return;
    //                }
    //            }

    //            insertLinks(t);
    //            observer.IsEmpty = false;
    //        }
    //    }

    //    public Task<bool> TryEnqueueAsync(Func<IQueueObserve<T>,T> func)
    //    {
    //        return TryEnqueueAsync((o) => Task.FromResult<T>(func(o)), CancellationToken.None);
    //    }
    //    public Task<bool> TryEnqueueAsync(Func<IQueueObserve<T>, T> func, CancellationToken token)
    //    {
    //        return TryEnqueueAsync((o) => Task.FromResult<T>(func(o)), token);
    //    }

    //    public async Task<bool> TryEnqueueAsync(Func<IQueueObserve<T>,Task<T>> func, CancellationToken token)
    //    {
    //        using (await locker.WaitAsync(token))
    //        {
    //            if (observer.IsEnqueueComplete)
    //                return false;

    //            T value = await func(this.observer);
    //            if (value == null)
    //                return false;

    //            insertLinks(value);
    //            observer.IsEmpty = false;
    //            return true;
    //        }
    //    }

    //    public Task<bool> TryEnqueueAsync(Func<IQueueObserve<T>, Task<T>> func)
    //    {
    //        return this.TryEnqueueAsync(func, CancellationToken.None);
    //    }

    //    void insertLinks(T t)
    //    {
    //        // wrap the value
    //        var node = new OrderedNode<T>(t);

    //        // rule out head and tail entries

    //        // see if we can append
    //        if (observer.PeekTail().CompareTo(t) <= 0)
    //        {
    //            this.queue.InsertAtTail(node);
    //            return;
    //        }

    //        // see if we can prepend
    //        if (observer.PeekHead().CompareTo(t) > 0)
    //        {
    //            this.queue.InsertAtHead(node);
    //            return;
    //        }

    //        // ok, now search for neighbors and insert links
    //        var cur = this.queue.Head;
    //        while (cur.Next != null && cur.Next.CompareTo(node) <= 0)
    //            cur = cur.Next;

    //        // insert where we stopped (cannot be head or tail)
    //        this.queue.InsertAfter(cur, node);
    //    }

    //    public async Task EnqueueAsync(T t)
    //    {
    //        await this.EnqueueAsync(t, CancellationToken.None);
    //    }

    //}

//    internal class QueueOrdered<T> : IOrderedList<T> where T : class, INodeC<T>
//    {
        //public override void InsertAfter(T cur, T node)
        //{
        //    if (node.CompareTo(cur) < 0)
        //        throw new InvalidOperationException("node is not less than current");

        //    base.InsertAfter(cur, node);
        //}

        //public override void InsertAtHead(T node)
        //{
        //    if (this.Head != null && node.CompareTo(this.Head) > 0)
        //        throw new InvalidOperationException("node is greater than head");

        //    base.InsertAtHead(node);
        //}

        //public override void InsertAtTail(T node)
        //{
        //    if (this.Tail != null && node.CompareTo(this.Tail) < 0)
        //        throw new InvalidOperationException("node is less than tail");

        //    base.InsertAtTail(node);
        //}

        //public override void InsertBefore(T cur, T node)
        //{
        //    if (node.CompareTo(cur) > 0)
        //        throw new InvalidOperationException("node is greater than current");

        //    base.InsertBefore(cur, node);
        //}

        ////public override void Add(T node)
        ////{
        ////    // special case for empty
        ////    if (this.Count == 0)
        ////        base.InsertAtTail(node);
        ////    // see if we can append
        ////    else if (this.Tail.CompareTo(node) <= 0)
        ////        base.InsertAtTail(node);
        ////    // see if we can prepend
        ////    else if (this.Head.CompareTo(node) >= 0)
        ////        base.InsertAtHead(node);
        ////    // else insert in order comparing bottom up
        ////    else
        ////        InsertInOrder(node);
        ////}

        //void InsertInOrder(T node)
        //{
        //        // ok, now search for neighbors and insert links
        //        INode cur = this.Tail;

        //        // we'll search from the end to begin, since most queues will be added near the end
        //        while (cur.Prev != null && ((T)cur.Prev).CompareTo(node) > 0)
        //            cur = cur.Prev;

        //        // insert where we stopped (cannot be head or tail)
        //        base.InsertBefore((T) cur, node);
        //}
}
