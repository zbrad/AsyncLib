using System;
using System.Threading.Tasks;
using System.Threading;

namespace ZBrad.AsyncLib
{
    /// <summary>
    /// Provides an implicit acquire/release wrapper for BinarySemaphore. 
    /// This enables the pattern:
    /// <code>
    /// AwaitLock foo;
    /// 
    /// method()
    /// {
    ///   using(await foo.WaitAsync())
    ///   {
    ///       // foo is locked during the scope of "using"
    ///   }
    ///   // foo is implicitly released on dispose
    /// }
    /// </code>
    /// </summary>
    public class AwaitLock
    {
        public struct Releaser : IDisposable
        {
            readonly AwaitLock locked;
            internal Releaser(AwaitLock b)
            {
                locked = b;
            }

            void IDisposable.Dispose()
            {
                if (locked != null)
                    locked.sem.Release();
            }
        }

        BinarySemaphore sem = new BinarySemaphore();
        Releaser releaser;
        static Releaser cancelledReleaser;

        public AwaitLock()
        {
            this.releaser = new Releaser(this);
        }

        static AwaitLock()
        {
            cancelledReleaser = new Releaser(null);
        }

        public async Task<Releaser> WaitAsync(CancellationToken token)
        {
            Task<bool> t = sem.WaitAsync(token);
            if (t.IsCompleted)
            {
                if (t.Result)
                    return this.releaser;
                return cancelledReleaser;
            }

            Task<Releaser> r = t.ContinueWith<Releaser>(lockComplete, this, token, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
            return await r;
        }

        Releaser lockComplete(Task<bool> t, object o)
        {
            if (!t.Result)
                throw new OperationCanceledException("lock failed");

            AwaitLock u = (AwaitLock)o;
            return new Releaser(u);
        }

        public Task<Releaser> WaitAsync()
        {
            return this.WaitAsync(CancellationToken.None);
        }

    }
}
