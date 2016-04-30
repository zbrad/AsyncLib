using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
using ZBrad.AsyncLib;
using System.Threading;
using System.Threading.Tasks;
using ZBrad.AsyncLib.Nodes;

namespace Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class WaitQueueTests
    {
        int item0 = 1000;
        int item1 = 2000;

        [TestMethod]
        public void IsEmpty_WhenEmpty_IsTrue()
        {
            var queue = new WaitQueue<int>();
            Assert.IsTrue(queue.IsEmptyAsync().ResultEx());
        }

        [TestMethod]
        public void IsEmpty_WithOneItem_IsFalse()
        {
            var queue = new WaitQueue<int>();
            queue.EnqueueAsync(item0).WaitEx();
            Assert.IsFalse(queue.IsEmptyAsync().ResultEx());
        }

        [TestMethod]
        public void IsEmpty_WithTwoItems_IsFalse()
        {
            var queue = new WaitQueue<int>();
            queue.EnqueueAsync(item0).WaitEx();
            queue.EnqueueAsync(item1).WaitEx();
            Assert.IsFalse(queue.IsEmptyAsync().ResultEx());
        }

        [TestMethod]
        public void Dequeue_WaitWithEnqCompletes_CompletesTask()
        {
            var queue = new WaitQueue<int>();
            var task0 = queue.DequeueAsync();
            Assert.IsFalse(task0.IsCompleted);
            var task1 = queue.EnqueueAsync(item0);
            task1.WaitEx();
            task0.WaitEx();
            Assert.IsTrue(task1.IsCompleted);
            Assert.IsTrue(task0.IsCompleted);
            Assert.AreEqual<int>(item0, task0.ResultEx());
        }

        [TestMethod]
        public void Dequeue_WithTwoItems_OnlyCompletesFirstItem()
        {
            Test.Async(async () =>
            {
                var queue = new WaitQueue<int>();
                var task1 = queue.EnqueueAsync(item0);
                var task2 = queue.EnqueueAsync(item1);
                await task1;
                var deq0 = await queue.DequeueAsync();
                Assert.AreEqual<int>(item0, deq0);
                await task2;
                Assert.AreEqual<int>(item1, await queue.PeekHeadAsync());
            });
        }

        [TestMethod]
        public void Dequeue_WithResult_CompletesWithResult()
        {
            var queue = new WaitQueue<int>();
            var task = queue.EnqueueAsync(item0);
            Test.Async(async () =>
            {
                var result0 = await queue.DequeueAsync();
                Assert.AreEqual<int>(item0, result0);
            });
        }

        [TestMethod]
        public void Dequeue_WithoutResult_CompletesWithDefaultResult()
        {
            var queue = new WaitQueue<int>();
            var task0 = queue.EnqueueAsync(item0);
            var result = queue.DequeueAsync().ResultEx();
            task0.Wait();
            Assert.AreEqual<int>(item0, result);
        }

        [TestMethod]
        public void Dequeue_EndEnqueue_IsComplete()
        {
            var queue = new WaitQueue<int>();
            var task1 = queue.EnqueueAsync(item0);
            var task2 = queue.EnqueueAsync(item1);

            Test.Async(async () =>
            {
                await task1;
                await task2;
                Assert.IsFalse(await queue.IsEmptyAsync());
                Assert.IsFalse(await queue.IsCompleteAsync());
                Assert.IsFalse(await queue.IsEndedAsync());
                await queue.EndEnqueueAsync();
                Assert.IsTrue(await queue.IsEndedAsync());
                Assert.IsFalse(await queue.IsEmptyAsync());
                Assert.IsFalse(await queue.IsCompleteAsync());
                while (queue.Count > 0)
                    await queue.DequeueAsync();
                Assert.IsTrue(await queue.IsCompleteAsync());
                Assert.IsTrue(await queue.IsEmptyAsync());
                Assert.IsTrue(await queue.IsEndedAsync());
            });

            Assert.IsTrue(task1.IsCompleted);
            Assert.IsTrue(task2.IsCompleted);
        }

        [TestMethod]
        public void Enqueue_Cancel()
        {
            var wq = new WaitQueue<int>();
            var queue = wq;
            var cts = new CancellationTokenSource();
            Test.Async(async () =>
            {
                // gets internal releaser for wait queue
                var releaser = (IDisposable)await wq.Locker.WaitAsync();

                var task = queue.EnqueueAsync(item0, cts.Token); // will block waiting for releaser
                Assert.IsFalse(task.IsCompleted);

                cts.Cancel();  // now cancel enqueue while it's blocked
                await AssertEx.ThrowsExceptionAsync<OperationCanceledException>(task);

                // clear the internal lock
                releaser.Dispose();

                Assert.IsTrue(await queue.IsEmptyAsync());
                Assert.IsTrue(task.IsCanceled);
            });
        }

        [TestMethod]
        public void Cancelled_Dequeue()
        {
            var queue = new WaitQueue<int>();
            var cts = new CancellationTokenSource();
            var task = queue.DequeueAsync(cts.Token);

            Test.Async(async () =>
            {
                await Task.Delay(2);
                Assert.AreEqual<int>(1, queue.WaitCount);
                cts.Cancel();
                await AssertEx.ThrowsExceptionAsync<OperationCanceledException>(task);
                Assert.IsTrue(await queue.IsEmptyAsync());
            });
        }

        [TestMethod]
        public void Cancelled_BeforeEnqueue_SynchronouslyFaultsTask()
        {
            var queue = new WaitQueue<int>();
            var cts = new CancellationTokenSource();
            cts.Cancel();
            var task = queue.EnqueueAsync(item0, cts.Token);
            Assert.
            Assert.IsTrue(task.IsCanceled);
            Assert.IsTrue(queue.IsEmptyAsync().ResultEx());
        }

        [TestMethod]
        public void Cancelled_BeforeEnqueue_RemovesTaskFromQueue()
        {
            var queue = new WaitQueue<int>();
            var cts = new CancellationTokenSource();
            cts.Cancel();
            var task = queue.DequeueAsync(cts.Token);
            Assert.IsTrue(task.IsCanceled);
            Assert.IsTrue(queue.IsEmptyAsync().ResultEx());
        }

    }
}
