using System;

using C = System.Collections;
using G = System.Collections.Generic;
using U = System.Collections.Concurrent;

using System.Threading;
using System.Threading.Tasks;
using ZBrad.AsyncLib.Collections;

namespace ZBrad.AsyncLib
{
    public class WaitQueue<N> : IAsyncQueue<N>, IWaitable where N : IEquatable<N>
    {
        AsyncQueue<N> queue = new AsyncQueue<N>();

        bool isEnded = false;

        bool isComplete = false;

        #region IAsyncQueue

        public IQueue<N> Queue { get { return queue.Queue; } }

        public AwaitLock Lock { get { return queue.Lock; } }

        public int Count { get { return queue.Count; } }

        public Task CopyTo(N[] array, int arrayIndex, CancellationToken token)
        {
            return queue.CopyTo(array, arrayIndex, token);
        }

        IAsyncEnumerator<N> IAsyncEnumerable<N>.GetAsyncEnumerator()
        {
            return queue.GetAsyncEnumerator();
        }

        #endregion

        public async Task<N> Peek(CancellationToken token)
        {
            var trial = await TryPeek(token);
            if (trial.Result)
                return trial.Value;

            return default(N);
        }

        /// <summary>
        /// peek will not block if there are no items on the queue
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<ITry<N>> TryPeek(CancellationToken token)
        {
            using (await queue.Lock.Wait(token))
            {
                if (isEnded || queue.Count == 0)
                    return Try<N>.False;

                return new Try<N>(queue.Queue.Peek());
            }
        }

        // we have to use list to allow cancel cleanup
        LinkQueue<Waiter<N>> waiters = new LinkQueue<Waiter<N>>();

        public int WaitCount { get { return waiters.Count; } }

        public async Task EndEnqueue(CancellationToken token)
        {
            using (await queue.Lock.Wait(token))
            {
                isEnded = true;
            }
        }

        public async Task<bool> IsEnded(CancellationToken token)
        {
            using (await queue.Lock.Wait(token))
            {
                return isEnded;
            }
        }

        public async Task<bool> IsComplete(CancellationToken token)
        {
            if (isComplete)
                return true;

            using (await queue.Lock.Wait(token))
            {
                if (isEnded && queue.Count == 0)
                {
                    isComplete = true;
                    return true;
                }

                return false;
            }
        }

        public async Task<bool> IsEmpty(CancellationToken token)
        {
            using (await queue.Lock.Wait(token))
            {
                return queue.Count == 0;
            }
        }

        public Task Enqueue(N item, CancellationToken token)
        {
            return TryEnqueue(item, token);
        }

        public async Task<bool> TryEnqueue(N item, CancellationToken token)
        {
            using (await queue.Lock.Wait(token))
            {
                // are we done adding?
                if (isEnded)
                    return false;

                // special case for havings waiters and no queue elements
                if (waiters.Count > 0 && queue.Count == 0)
                {
                    var w = waiters.Dequeue();
                    w.Completed(item);
                    return true;
                }

                queue.Queue.Enqueue(item);
                completeWaiters();
            }

            return true;
        }

        void completeWaiters()
        {
            while (waiters.Count > 0 && queue.Count > 0)
            {
                var w = waiters.Dequeue();
                var x = queue.Queue.Dequeue();
                w.Completed(x);
            }
        }

        public async Task<N> Dequeue(CancellationToken token)
        {
            Waiter<N> waiter = null;

            using (await queue.Lock.Wait(token))
            {
                if (isComplete)
                    return default(N);

                if (waiters.Count == 0 && queue.Count > 0)
                    return queue.Queue.Dequeue();

                waiter = new Waiter<N>(token);

                if (token != CancellationToken.None)
                    waiter.OnCancel += Waiter_OnCancel;

                waiters.Enqueue(waiter);
                completeWaiters();
            }

            var value = await waiter;
            return value;
        }

        private void Waiter_OnCancel(Waiter<N> w)
        {
            waiters.Remove(w);
        }

        public Task TrimExcess(CancellationToken token)
        {
            return queue.TrimExcess(token);
        }
    }
}
