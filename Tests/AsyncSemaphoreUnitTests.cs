using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using ZBrad.AsyncLib;

namespace Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class BinarySemaphoreTests
    {
        static CancellationToken none = CancellationToken.None;

        [TestMethod]
        public void WaitAsync_IsCompleted()
        {
            Test.Async(async () =>
            {
                var semaphore = new BinarySemaphore();
                Assert.IsFalse(semaphore.IsLocked);
                Assert.AreEqual<int>(0, semaphore.WaitCount);
                var task0 = semaphore.Wait(none);
                var result0 = await task0;
                Assert.IsTrue(result0);
                Assert.IsTrue(semaphore.IsLocked);
                Assert.AreEqual<int>(0, semaphore.WaitCount);
                var task1 = semaphore.Wait(none);
                await Task.Delay(5);  // need brief pause, or waitcount might not be updated
                Assert.AreEqual<int>(1, semaphore.WaitCount);
                await AssertEx.NeverCompletesAsync(task1);
            });
        }

        [TestMethod]
        public void WaitAsync_PreCancelled_FailsSynchronously()
        {
            var semaphore = new BinarySemaphore();
            Assert.IsFalse(semaphore.IsLocked);
            Assert.AreEqual<int>(0, semaphore.WaitCount);

            var token = new CancellationToken(true);
            var task0 = semaphore.Wait(token);

            // should throw an exception
            AssertEx.ThrowsException<AggregateException>(() => { var result = task0.Result; });

            Assert.AreEqual<int>(0, semaphore.WaitCount);
            Assert.IsTrue(task0.IsCompleted);
            Assert.IsTrue(task0.IsFaulted);
        }

        [TestMethod]
        public void WaitAsync_Cancel_WithWaiters()
        {
            Test.Async(async () =>
            {
                var semaphore = new BinarySemaphore();
                Assert.IsFalse(semaphore.IsLocked);
                Assert.AreEqual<int>(0, semaphore.WaitCount);

                var task0 = semaphore.Wait(none);
                var result0 = await task0;
                Assert.IsTrue(result0);
                var id0 = task0.Id;
                Console.WriteLine("task0 id0=" + id0);

                var cts1 = new CancellationTokenSource();
                var task1 = semaphore.Wait(cts1.Token);
                Assert.IsFalse(task1.IsCompleted);
                var id1 = task1.Id;
                Console.WriteLine("task1 id1=" + id1);

                // no token for task2
                var task2 = semaphore.Wait(none);
                Assert.IsFalse(task2.IsCompleted);
                var id2 = task2.Id;
                Console.WriteLine("task2 id2=" + id2);

                // now have 2 waiting tasks
                Assert.AreEqual<int>(2, semaphore.WaitCount);

                // cancel the first waiter
                cts1.Cancel();

                try
                {
                    // for for it to throw
                    await task1;
                }
                catch (Exception e)
                {
                    Assert.IsInstanceOfType(e, typeof(OperationCanceledException));
                }

                // validate cancellation
                Assert.IsTrue(task1.IsCanceled);

                // validate that lock is still held
                Assert.IsTrue(semaphore.IsLocked);

                // should now be only 1 waiter
                Assert.AreEqual<int>(1, semaphore.WaitCount);

                // now release to allow task2 to complete
                semaphore.Release();
                await task2;

                // now validate lock held and no waiters
                Assert.IsTrue(task2.IsCompleted);
                Assert.IsTrue(semaphore.IsLocked);
                Assert.AreEqual<int>(0, semaphore.WaitCount);

                // now release lock
                semaphore.Release();
                Assert.IsFalse(semaphore.IsLocked);
            });
        }

        [TestMethod]
        public void Release_WithoutWaiters_IncrementsCount()
        {
            var semaphore = new BinarySemaphore();
            Assert.IsFalse(semaphore.IsLocked);
            var task = semaphore.Wait(none);
            Assert.IsTrue(task.Result);
            Assert.AreEqual<int>(0, semaphore.WaitCount);
            Assert.IsTrue(semaphore.IsLocked);
            Assert.IsTrue(task.IsCompleted);
            semaphore.Release();
            Assert.IsFalse(semaphore.IsLocked);
        }

        [TestMethod]
        public void Release_WithWaiters_ReleasesWaiters()
        {
            Test.Async(releasesWaiters);
        }

        async Task releasesWaiters()
        {
            var semaphore = new BinarySemaphore();
            Assert.AreEqual<int>(0, semaphore.WaitCount);
            var wait0 = await semaphore.Wait(none);
            Assert.IsTrue(wait0);
            Assert.AreEqual<int>(0, semaphore.WaitCount);
            Assert.IsTrue(semaphore.IsLocked);

            var task = semaphore.Wait(none);
            Assert.IsFalse(task.IsCompleted);
            Assert.AreEqual<int>(1, semaphore.WaitCount);
            semaphore.Release();
            var wait1 = await task;
            Assert.IsTrue(wait1);
            Assert.IsTrue(semaphore.IsLocked);
            Assert.AreEqual<int>(0, semaphore.WaitCount);
        }
    }
}
