using System;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using ZBrad.AsyncLib.Nodes;

namespace ZBrad.AsyncLib
{
    internal delegate void CancelEvent<T>(Waiter<T> w);

    public class Waiter<T> : ILink, ICriticalNotifyCompletion, IDisposable, IEquatable<Waiter<T>>
    {
        public ILink Prev { get; set; }

        public ILink Next { get; set; }

        static long sequence = 0;
        static readonly CancellationTokenRegistration EmptyRegistration = default(CancellationTokenRegistration);
        Action nextAction = null;
        long id = Interlocked.Increment(ref sequence);

        CancellationTokenRegistration userTokenReg;
        CancellationTokenRegistration waiterTokenReg;

        T result;

        public long Id { get { return id; } }

        public bool IsCompleted { get; private set; }

        public bool IsCancelled { get; private set; }

        internal event CancelEvent<T> OnCancel;

        Waiter() 
        {
            this.result = default(T);
            this.IsCompleted = false;
            this.IsCancelled = false;
        }

        public Waiter(CancellationToken token) : this()
        {
            token.ThrowIfCancellationRequested();

            if (token != CancellationToken.None)
                userTokenReg = token.Register(this.onUserCancel, token, true);
        }

        public bool Equals(Waiter<T> other)
        {
            if (other == null)
                return false;

            return id == other.id;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            return this.Equals(obj as Waiter<T>);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        void onUserCancel(object otoken)
        {
            // if we have already completed, then we ignore the cancel
            if (IsCompleted)
                return;

            IsCancelled = true;
            IsCompleted = true;

            var cancel = this.OnCancel;
            if (cancel != null)
                cancel(this);

            Task.Run(this.nextAction);
        }

        public void Completed(T result)
        {
            this.result = result; 
            this.IsCompleted = true;

            Task.Run(this.nextAction);
        }

        public Waiter<T> GetAwaiter() { return this; }

        public T GetResult()
        {
            if (this.IsCancelled)
                throw new OperationCanceledException("waiter cancelled");

            return result;
        }

        [SecurityCritical]
        void ICriticalNotifyCompletion.UnsafeOnCompleted(Action continuation)
        {
            this.nextAction = continuation;
        }

        void INotifyCompletion.OnCompleted(Action continuation)
        {
            throw new NotImplementedException();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (!userTokenReg.Equals(EmptyRegistration))
                        userTokenReg.Dispose();
                    userTokenReg = EmptyRegistration;

                    if (!waiterTokenReg.Equals(EmptyRegistration))
                        waiterTokenReg.Dispose();
                    waiterTokenReg = EmptyRegistration;
                }

                disposedValue = true;
            }
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}