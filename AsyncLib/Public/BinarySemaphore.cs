using System;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using ZBrad.AsyncLib.Collections;

namespace ZBrad.AsyncLib
{
    /// <summary>
    /// simplified binary semaphore, based on semaphore slim implementation:
    /// http://referencesource.microsoft.com/#mscorlib/system/threading/SemaphoreSlim.cs 
    /// rationale for local implementation is that it allows internal assembly optimizations (inlining)
    /// </summary>
    public sealed class BinarySemaphore
    {
        SpinLock spinner = new SpinLock();
        LinkedList<Waiter<bool>> waiters = new LinkedList<Waiter<bool>>();

        public bool IsLocked { get; private set; }

        public int WaitCount { get { return waiters.Count; } }

        public Task<bool> WaitAsync(CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                return TaskEx.FaultedBool;
            }

            return wait(token);
        }

        async Task<bool> wait(CancellationToken token)
        { 
            Waiter<bool> waiter = null;
            bool lockTaken = false;
            try
            {
                spinner.Enter(ref lockTaken);

                if (!this.IsLocked && this.waiters.Count == 0)
                {
                    this.IsLocked = true;
                    return true;
                }

                waiter = new Waiter<bool>(token);
                if (token != CancellationToken.None)
                    waiter.OnCancel += Waiter_OnCancel;

                waiters.InsertAtTail(waiter);
            }
            finally
            {
                if (lockTaken)
                    spinner.Exit(false);
            }

            if (waiter == null)
                return false;

            bool hasLock = await waiter;
            return hasLock;
        }

        private void Waiter_OnCancel(Waiter<bool> w)
        {
            bool spinTaken = false;
            try
            {
                spinner.Enter(ref spinTaken);
                waiters.Remove(w);
            }
            finally
            {
                if (spinTaken)
                    spinner.Exit(false);
            }

            w.Completed(false);
        }

        public Task<bool> WaitAsync()
        {
            return this.WaitAsync(CancellationToken.None);
        }

        public void Release()
        {
            bool spinTaken = false;
            Waiter<bool> w = null;

            try
            {
                spinner.Enter(ref spinTaken);
                if (!this.IsLocked)
                    return;

                this.IsLocked = false;

                // check if any waiters
                if (waiters.Count > 0)
                {
                    // get awaiter
                    w = waiters.RemoveFromHead();

                    // perform a lock operation if we dequeued an awaiter
                    this.IsLocked = true;
                }
            }
            finally
            {
                if (spinTaken)
                    spinner.Exit(false);
            }

            // if waiter was removed, complete it, outside of spinlock
            if (w != null)
                w.Completed(IsLocked);
        }
    }
}
