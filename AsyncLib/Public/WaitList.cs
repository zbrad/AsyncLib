using System;
using C = System.Collections;
using G = System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZBrad.AsyncLib.Collections;
using ZBrad.AsyncLib.Nodes;

namespace ZBrad.AsyncLib
{
    public class WaitList<N> : IValueAsyncCollection<N>, IWaitable where N : IEquatable<N>
    {
        LinkedList<N> list = new LinkedList<N>();
        bool isEnded = false;

        // we have to use list to allow cancel cleanup
        LinkedList<RemovalWaiter> waiters = new LinkedList<RemovalWaiter>();

        public int WaitCount { get { return waiters.Count; } }

        AwaitLock llock = new AwaitLock();
        public AwaitLock Locker { get { return this.llock; } }

        public long Version { get { return list.Version; } }

        IValue<N> IValueAsyncCollection<N>.Root { get { return list.Root; } }      

        public int Count { get { return list.Count; } }

        public bool IsReadOnly { get { return false; } }

        public Task<bool> InsertAtHeadAsync(N item) { return InsertAtHeadAsync(item, CancellationToken.None); }

        public Task<bool> InsertAtTailAsync(N item) { return InsertAtTailAsync(item, CancellationToken.None); }

        public Task<N> RemoveFromHeadAsync() { return RemoveFromHeadAsync(CancellationToken.None); }

        public Task<N> RemoveFromTailAsync() { return RemoveFromTailAsync(CancellationToken.None); }

        public Task<bool> RemoveAsync(N value) { return RemoveAsync(value, CancellationToken.None); }

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
            return remove(() => list.RemoveFromHead(), token);
        }

        public Task<N> RemoveFromTailAsync(CancellationToken token)
        {
            return remove(() => list.RemoveFromTail(), token);
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

        public Task EndEnqueueAsync()
        {
            return EndEnqueueAsync(CancellationToken.None);
        }

        public async Task EndEnqueueAsync(CancellationToken token)
        { 
            using (await this.Locker.WaitAsync(token))
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

        public Task ClearAsync()
        {
            return ClearAsync(CancellationToken.None);
        }

        public async Task ClearAsync(CancellationToken token)
        {
            using (await this.Locker.WaitAsync(token))
            {
                list.Clear();
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

        public Task CopyToAsync(N[] array, int arrayIndex)
        {
            return CopyToAsync(array, arrayIndex, CancellationToken.None);
        }

        public async Task CopyToAsync(N[] array, int arrayIndex, CancellationToken token)
        {
            using (await this.Locker.WaitAsync(token))
            {
                list.CopyTo(array, arrayIndex);
            }
        }

        #endregion


        #region async enumerator

        public IValueAsyncEnumerator<N> GetAsyncEnumerator()
        {
            return new NodeEnumAsync<N>(this);
        }

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
                    var w = waiters.RemoveFromHead();
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
                // execute the function to get the item and complete with func returned item
                var w = waiters.RemoveFromHead();
                w.Completed(w.Func());
            }
        }

        async Task<N> remove(Func<N> remover, CancellationToken token)
        {
            RemovalWaiter waiter = null;

            using (await this.Locker.WaitAsync(token))
            {
                if (isComplete())
                    return default(N);

                if (waiters.Count == 0 && list.Count > 0)
                    return remover();

                waiter = new RemovalWaiter(remover, token);

                if (token != CancellationToken.None)
                    waiter.OnCancel += waiterOnCancel;
                waiters.InsertAtTail(waiter);

                completeWaiters();
            }

            N item = await waiter;
            return item;
        }

        void waiterOnCancel(Waiter<N> w)
        {
            Task.Run(async () =>
            {
                using (await this.Locker.WaitAsync())
                {
                    waiters.Remove((RemovalWaiter) w);
                }
            });
        }

        #endregion

        #region private classes

        class RemovalWaiter : Waiter<N>, IEquatable<RemovalWaiter>
        {
            public Func<N> Func { get; private set; }
            public RemovalWaiter(Func<N> func, CancellationToken token) : base(token) { this.Func = func; }

            public bool Equals(RemovalWaiter other)
            {
                return base.Equals(other);
            }
        }

        #endregion
    }
}
