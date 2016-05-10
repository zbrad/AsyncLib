using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
using ZBrad.AsyncLib;
using System.Threading;
using System.Threading.Tasks;

namespace Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class WaitQueueTests
    {
        static CancellationToken none = CancellationToken.None;
        int item0 = 1000;
        int item1 = 2000;

        [TestMethod]
        public void IsEmpty_WhenEmpty_IsTrue()
        {
            var queue = new WaitQueue<int>();
            Assert.IsTrue(queue.IsEmpty(none).ResultEx());
        }

        [TestMethod]
        public void IsEmpty_WithOneItem_IsFalse()
        {
            var queue = new WaitQueue<int>();
            queue.Enqueue(item0, none).WaitEx();
            Assert.IsFalse(queue.IsEmpty(none).ResultEx());
        }

        [TestMethod]
        public void IsEmpty_WithTwoItems_IsFalse()
        {
            var queue = new WaitQueue<int>();
            queue.Enqueue(item0, none).WaitEx();
            queue.Enqueue(item1, none).WaitEx();
            Assert.IsFalse(queue.IsEmpty(none).ResultEx());
        }

        [TestMethod]
        public void Dequeue_WaitWithEnqCompletes_CompletesTask()
        {
            var queue = new WaitQueue<int>();
            var task0 = queue.Dequeue(none);
            Assert.IsFalse(task0.IsCompleted);
            var task1 = queue.Enqueue(item0, none);
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
                var task1 = queue.Enqueue(item0, none);
                var task2 = queue.Enqueue(item1, none);
                await task1;
                var deq0 = await queue.Dequeue(none);
                Assert.AreEqual<int>(item0, deq0);
                await task2;
                Assert.AreEqual<int>(item1, await queue.Peek(none));
            });
        }

        [TestMethod]
        public void Dequeue_WithResult_CompletesWithResult()
        {
            var queue = new WaitQueue<int>();
            var task = queue.Enqueue(item0, none);
            Test.Async(async () =>
            {
                var result0 = await queue.Dequeue(none);
                Assert.AreEqual<int>(item0, result0);
            });
        }

        [TestMethod]
        public void Dequeue_WithoutResult_CompletesWithDefaultResult()
        {
            var queue = new WaitQueue<int>();
            var task0 = queue.Enqueue(item0, none);
            var result = queue.Dequeue(none).ResultEx();
            task0.Wait();
            Assert.AreEqual<int>(item0, result);
        }

        [TestMethod]
        public void Dequeue_EndEnqueue_IsComplete()
        {
            var queue = new WaitQueue<int>();
            var task1 = queue.Enqueue(item0, none);
            var task2 = queue.Enqueue(item1, none);

            Test.Async(async () =>
            {
                await task1;
                await task2;
                Assert.IsFalse(await queue.IsEmpty(none));
                Assert.IsFalse(await queue.IsComplete(none));
                Assert.IsFalse(await queue.IsEnded(none));
                await queue.EndEnqueue(none);
                Assert.IsTrue(await queue.IsEnded(none));
                Assert.IsFalse(await queue.IsEmpty(none));
                Assert.IsFalse(await queue.IsComplete(none));
                while (queue.Count > 0)
                    await queue.Dequeue(none);
                Assert.IsTrue(await queue.IsComplete(none));
                Assert.IsTrue(await queue.IsEmpty(none));
                Assert.IsTrue(await queue.IsEnded(none));
            });

            Assert.IsTrue(task1.IsCompleted);
            Assert.IsTrue(task2.IsCompleted);
        }

        [TestMethod]
        public void Cancelled_Enqueue()
        {
            var wq = new WaitQueue<int>();
            var queue = wq;
            var cts = new CancellationTokenSource();
            Test.Async(async () =>
            {
                // gets internal releaser for wait queue
                var releaser = (IDisposable)await wq.Lock.Wait(none);

                var task = queue.Enqueue(item0, cts.Token); // will block waiting for releaser
                Assert.IsFalse(task.IsCompleted);

                cts.Cancel();  // now cancel enqueue while it's blocked
                await AssertEx.ThrowsExceptionAsync<OperationCanceledException>(task);

                // clear the internal lock
                releaser.Dispose();

                Assert.IsTrue(await queue.IsEmpty(none));
                Assert.IsTrue(task.IsCanceled);
            });
        }

        [TestMethod]
        public void Cancelled_Dequeue()
        {
            var queue = new WaitQueue<int>();
            var cts = new CancellationTokenSource();
            var task0 = queue.Dequeue(cts.Token);

            Task.Run(async () =>
            {
                // wait until dequeue request gets on waitlist
                while (queue.WaitCount == 0)
                    await Task.Delay(10);
                Assert.AreEqual<int>(1, queue.WaitCount);
            }).GetAwaiter().GetResult();

            cts.Cancel();
            AssertEx.ThrowsExceptionAsync<OperationCanceledException>(task0).GetAwaiter().GetResult();
            var isEmpty = queue.IsEmpty(none).GetAwaiter().GetResult();
            Assert.IsTrue(isEmpty);
        }

        [TestMethod]
        public void Cancelled_BeforeEnqueue_Faults()
        {
            var queue = new WaitQueue<int>();
            var cts = new CancellationTokenSource();
            cts.Cancel();
            var task = queue.Enqueue(item0, cts.Token);
            AssertEx.ThrowsExceptionAsync<OperationCanceledException>(task).GetAwaiter().GetResult();
            Assert.IsTrue(task.IsCanceled);
            Assert.IsTrue(queue.IsEmpty(none).ResultEx());
        }

        [TestMethod]
        public void Cancelled_BeforeDequeue_Faults()
        {
            var queue = new WaitQueue<int>();
            var cts = new CancellationTokenSource();
            cts.Cancel();
            var task = queue.Dequeue(cts.Token);
            AssertEx.ThrowsExceptionAsync<OperationCanceledException>(task).GetAwaiter().GetResult();
            Assert.IsTrue(task.IsCanceled);
            Assert.IsTrue(queue.IsEmpty(none).ResultEx());
        }

    }
}
