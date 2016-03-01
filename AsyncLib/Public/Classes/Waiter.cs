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
        CancellationTokenRegistration registration;

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
            var id = Task.CurrentId;
            Console.WriteLine("waiter taskid=" + id);
            if (token != CancellationToken.None)
                registration = token.Register(this.cancel, token, true);
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

        void cancel(object otoken)
        {
            var id = Task.CurrentId;
            Console.WriteLine("cancel taskid=" + id);
            if (IsCompleted)
                return;

            IsCancelled = true;
            var cancel = this.OnCancel;
            if (cancel != null)
                cancel(this);
        }

        //public Waiter(T result)
        //{
        //    this.Result = result;
        //    this.IsCompleted = true;
        //}

        //public void Completed()
        //{
        //    Completed(default(T));
        //}

        public void Completed(T result)
        {
            this.Result = result; 
            this.IsCompleted = true;

            this.nextAction();
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
                    if (registration != EmptyRegistration)
                        registration.Dispose();
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