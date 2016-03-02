using System;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace ZBrad.AsyncLib
{
    public delegate void CancelEvent<T>(Waiter<T> w);

    public class Waiter<T> : INode, IEquatable<Waiter<T>>, ICriticalNotifyCompletion, IDisposable
    {
        static long sequence = 0;
        static readonly CancellationTokenRegistration EmptyRegistration = default(CancellationTokenRegistration);
        Action nextAction = null;
        long id = Interlocked.Increment(ref sequence);

        CancellationTokenRegistration userTokenReg;
        CancellationTokenRegistration waiterTokenReg;
        CancellationTokenSource cts = new CancellationTokenSource();

        public CancellationToken Token { get { return cts.Token; } }

        public long Id { get { return id; } }

        public bool IsCompleted { get; private set; }

        public T Result { get; private set; }

        public bool IsCancelled { get; private set; }

        INode INode.Prev { get; set; }

        INode INode.Next { get; set; }

        public event CancelEvent<T> OnCancel;

        Waiter() 
        {
            this.Result = default(T);
            this.IsCompleted = false;
            this.IsCancelled = false;
        }

        public Waiter(CancellationToken token) : this()
        {
            //var id = Task.CurrentId;
            //log.Info("waiter taskid=" + id);

            if (token != CancellationToken.None)
                userTokenReg = token.Register(this.onUserCancel, token, true);

            waiterTokenReg = cts.Token.Register(this.onWaiterCancel, cts.Token, true);
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
            if (IsCompleted)
                return;

            cts.Cancel();
        }

        void onWaiterCancel(object otoken)
        {
            IsCancelled = true;
            IsCompleted = true;

            var cancel = this.OnCancel;
            if (cancel != null)
                cancel(this);
        }

        public void Completed(T result)
        {
            if (cts.Token.IsCancellationRequested)
                cts.Token.ThrowIfCancellationRequested();

            this.Result = result; 
            this.IsCompleted = true;
            this.nextAction();
        }

        public void Cancel()
        {
            cts.Cancel();
        }

        public Waiter<T> GetAwaiter() { return this; }

        public T GetResult() { return this.Result; }

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