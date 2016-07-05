using System;
using System.Threading;
using System.Threading.Tasks;

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
    ///   using(await foo.Wait(token))
    ///   {
    ///       // foo is locked during the scope of "using"
    ///   }
    ///   // foo is implicitly released on dispose
    /// }
    /// </code>
    /// </summary>
    public sealed class AwaitLock
    {
        static Task<Releaser> emptyReleaser;
        static Task<Releaser> faultReleaser = TaskEx.CancelFault<Releaser>();

        BinarySemaphore sem = new BinarySemaphore();
        Task<Releaser> releaser;

        public AwaitLock()
        {
            this.releaser = Task.FromResult<Releaser>(new Releaser(this));
        }

        static AwaitLock()
        {
            emptyReleaser = Task.FromResult<Releaser>(new Releaser(null));
        }

        public Task<Releaser> Wait(CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return faultReleaser;

            Task<bool> t = sem.Wait(token);

            // test for synchronous completion
            if (t.IsCompleted)
            {
                // if we had the lock, we'll release it at dispose
                if (t.Result)
                    return this.releaser;
                return emptyReleaser;
            }

            var r = t.ContinueWith<Releaser>(lockComplete, this, token, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
            return continueWait(r, token);
        }

        async Task<Releaser> continueWait(Task<Releaser> t, CancellationToken token)
        {
            try
            {
                var releaser = await t;
                return releaser;
            }
            catch (TaskCanceledException e)
            {
                throw new OperationCanceledException("await was cancelled", e, token);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        Releaser lockComplete(Task<bool> t, object o)
        {
            if (!t.Result)
                throw new OperationCanceledException("lock failed");

            AwaitLock u = (AwaitLock)o;
            return new Releaser(u);
        }

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
    }
}
