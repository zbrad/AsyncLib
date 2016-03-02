using System;
using C = System.Collections;
using G = System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZBrad.AsyncLib.Public.Classes
{
    public class WaitList<N> : IListAsync<N>, IWaitable where N : INode, IEquatable<N>
    {
        NodeList<N> list = new NodeList<N>();
        bool isEnded = false;
        static Task<N> Cancelled { get { return (Task<N>)TaskEx.Cancelled; } }

        // we have to use list to allow cancel cleanup
        NodeQueue<Waiter<N>> waiters = new NodeQueue<Waiter<N>>();

        public int WaitCount { get { return waiters.Count; } }

        AwaitLock llock = new AwaitLock();
        public AwaitLock Locker { get { return this.llock; } }

        public long Version { get { return list.Version; } }

        public INode Root { get { return list.Root; } }

        public int Count { get { return list.Count; } }

        bool G.ICollection<N>.IsReadOnly { get { return false; } }

        public Task<bool> InsertAtHeadAsync(N item) { return InsertAtHeadAsync(item, CancellationToken.None); }

        public Task<bool> InsertAtTailAsync(N item) { return InsertAtTailAsync(item, CancellationToken.None); }

        public Task<N> RemoveFromHeadAsync() { return RemoveFromHeadAsync(CancellationToken.None); }

        public Task<N> RemoveFromTailAsync() { return RemoveFromTailAsync(CancellationToken.None); }

        public Task<bool> InsertBeforeAsync(N item, N node) { return InsertBeforeAsync(item, node, CancellationToken.None); }

        public Task<bool> InsertAfterAsync(N item, N node) { return InsertAfterAsync(item, node, CancellationToken.None); }

        public Task<bool> RemoveAsync(N node) { return RemoveAsync(node, CancellationToken.None); }

        public Task<bool> InsertBeforeAsync(N item, N node, CancellationToken token)
        {
            return insert(item, (x) => list.InsertBefore(item, node), token);
        }

        public Task<bool> InsertAfterAsync(N item, N node, CancellationToken token)
        {
            return insert(item, (x) => list.InsertAfter(item, node), token);
        }

        public Task<bool> InsertAtHeadAsync(N item, CancellationToken token)
        {
            return insert(item, (x) => list.InsertAtHead(x), token);
        }

        public Task<bool> InsertAtTailAsync(N item, CancellationToken token)
        {
            return insert(item, (x) => list.InsertAtTail(x), token);
        }

        public Task<N> RemoveFromHeadAsync(CancellationToken token)
        {
            return remove<HeadWaiter>(() => list.RemoveFromHead(), token);
        }

        public Task<N> RemoveFromTailAsync(CancellationToken token)
        {
            return remove<TailWaiter>(() => list.RemoveFromTail(), token);
        }


        public async Task<bool> RemoveAsync(N node, CancellationToken token)
        {
            using (await this.Locker.WaitAsync(token))
            {
                return list.Remove(node);
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

        public async Task<bool> IsEmptyAsync(CancellationToken token)
        {
            using (await this.Locker.WaitAsync(token))
            {
                return list.Count == 0;
            }
        }

        public Task<bool> IsEmptyAsync()
        {
            return this.IsEmptyAsync(CancellationToken.None);
        }


        #region INodeCollectionAsync methods

        public Task<bool> AddAsync(N item)
        {
            return AddAsync(item, CancellationToken.None);
        }

        public Task<bool> AddAsync(N item, CancellationToken token)
        {
            return InsertAtTailAsync(item, token);
        }

        public Task<bool> ClearAsync()
        {
            return ClearAsync(CancellationToken.None);
        }

        public async Task<bool> ClearAsync(CancellationToken token)
        {
            using (await this.Locker.WaitAsync(token))
            {
                if (list.Count == 0)
                    return true;

                list.Clear();
                return true;
            }
        }

        public Task<bool> ContainsAsync(N item)
        {
            return ContainsAsync(item, CancellationToken.None);
        }

        public async Task<bool> ContainsAsync(N item, CancellationToken token)
        {
            using (await this.Locker.WaitAsync(token))
            {
                return list.Contains(item);
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
                list.CopyTo(array, arrayIndex);
                return true;
            }
        }

        #endregion

        #region ICollection methods

        void G.ICollection<N>.Add(N item) { throw new NotImplementedException("use InsertAsync methods"); }

        void G.ICollection<N>.Clear() { throw new NotImplementedException("use ClearAsync methods"); }

        bool G.ICollection<N>.Contains(N item) { throw new NotImplementedException("use ContainsAsync methods"); }

        void G.ICollection<N>.CopyTo(N[] array, int arrayIndex) { throw new NotImplementedException("use CopyToAsync methods"); }

        bool G.ICollection<N>.Remove(N item) { throw new NotImplementedException("use RemoveAsync methods"); }

        #endregion

        #region INodeNumerableAsync, INodeEnumerable, G.IEnumerable<N>, C.IEnumerable

        public IAsyncEnumerator<N> GetAsyncEnumerator()
        {
            return new NodesEnumAsync<N>(this);
        }

        G.IEnumerator<N> G.IEnumerable<N>.GetEnumerator()
        {
            return new SynchronousEnumerator<N>(this.GetAsyncEnumerator());
        }

        C.IEnumerator C.IEnumerable.GetEnumerator() { return ((G.IEnumerable<N>)this).GetEnumerator(); }

        #endregion

        #region private methods

        bool isComplete()
        {
            if (isEnded && list.Count == 0)
                return true;
            return false;
        }

        async Task<bool> insert(N item, Action<N> action, CancellationToken token)
        {
            using (await this.Locker.WaitAsync(token))
            {
                // are we done adding?
                if (isEnded)
                    return false;

                // special case for new item
                if (list.Count == 0 && waiters.Count > 0)
                {
                    // if only item, and already waiters, then just give first waiter this item
                    // doesn't matter if head or tail
                    var w = waiters.Dequeue();
                    w.Completed(item);
                    return true;
                }

                action(item);

                completeWaiters();
                return true;
            }
        }

        void completeWaiters()
        {
            while (waiters.Count > 0 && list.Count > 0)
            {
                // get type of waiter and complete with appropriate item
                var w = waiters.Dequeue();
                if (w is HeadWaiter)
                    w.Completed(list.RemoveFromHead());
                else if (w is TailWaiter)
                    w.Completed(list.RemoveFromTail());
                else
                    throw new InvalidOperationException("unknown waiter type");
            }
        }

        async Task<N> remove<W>(Func<N> remover, CancellationToken token) where W : Waiter<N>
        {
            Waiter<N> waiter = null;

            using (await this.Locker.WaitAsync(token))
            {
                if (isComplete())
                    return default(N);

                if (waiters.Count == 0 && list.Count > 0)
                    return remover();

                waiter = addWaiter<W>(token);
                completeWaiters();
            }

            N item = await waiter;
            return item;
        }

        Waiter<N> addWaiter<W>(CancellationToken token)
        {
            Waiter<N> waiter = null;
            if (typeof(W) == typeof(HeadWaiter))
            {
                waiter = new HeadWaiter(token);
            }
            else if (typeof(W) == typeof(TailWaiter))
            {
                waiter = new TailWaiter(token);
            }
            else
                throw new InvalidOperationException("invalid waiter type");

            if (token != CancellationToken.None)
                waiter.OnCancel += waiterOnCancel;

            waiters.Enqueue(waiter);
            return waiter;
        }

        void waiterOnCancel(Waiter<N> w)
        {
            Task.Run(async () =>
            {
                using (await this.Locker.WaitAsync())
                {
                    waiters.Remove(w);
                }
            });
        }

        #endregion

        #region private classes

        class HeadWaiter : Waiter<N>
        {
            public HeadWaiter(CancellationToken token) : base(token) { }
        }

        class TailWaiter : Waiter<N>
        {
            public TailWaiter(CancellationToken token) : base(token) { }
        }

        #endregion
    }
}
