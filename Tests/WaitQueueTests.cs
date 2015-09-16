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
        ItemNode<int> item0 = new ItemNode<int> { Item = 1000 };
        ItemNode<int> item1 = new ItemNode<int> { Item = 2000 };

        [TestMethod]
        public void IsEmpty_WhenEmpty_IsTrue()
        {
            var queue = new WaitQueue<ItemNode<int>>() as IWaitQueue<ItemNode<int>>;
            Assert.IsTrue(queue.IsEmpty().Result);
        }

        [TestMethod]
        public void IsEmpty_WithOneItem_IsFalse()
        {
            var queue = new WaitQueue<ItemNode<int>>() as IWaitQueue<ItemNode<int>>;
            queue.Enqueue(item0).Wait();
            Assert.IsFalse(queue.IsEmpty().Result);
        }

        [TestMethod]
        public void IsEmpty_WithTwoItems_IsFalse()
        {
            var queue = new WaitQueue<ItemNode<int>>() as IWaitQueue<ItemNode<int>>;
            queue.Enqueue(item0).Wait();
            queue.Enqueue(item1).Wait();
            Assert.IsFalse(queue.IsEmpty().Result);
        }

        [TestMethod]
        public void Dequeue_WaitWithEnqCompletes_CompletesTask()
        {
            var queue = new WaitQueue<ItemNode<int>>() as IWaitQueue<ItemNode<int>>;
            var task0 = queue.Dequeue();
            Assert.IsFalse(task0.IsCompleted);
            var task1 = queue.Enqueue(item0);
            task1.Wait();
            task0.Wait();
            Assert.IsTrue(task1.IsCompleted);
            Assert.IsTrue(task0.IsCompleted);
            Assert.AreEqual<ItemNode<int>>(item0, task0.Result);
        }

        [TestMethod]
        public void Dequeue_WithTwoItems_OnlyCompletesFirstItem()
        {
            Test.Async(async () =>
            {
                var queue = new WaitQueue<ItemNode<int>>() as IWaitQueue<ItemNode<int>>;
                var task1 = queue.Enqueue(item0);
                var task2 = queue.Enqueue(item1);
                queue.Dequeue().Dispose();
                Assert.IsTrue(task1.IsCompleted);
                await task2;
                Assert.AreEqual<ItemNode<int>>(item1, queue.PeekHead());
            });
        }

        [TestMethod]
        public void Dequeue_WithResult_CompletesWithResult()
        {
            var queue = new WaitQueue<ItemNode<int>>() as IWaitQueue<ItemNode<int>>;
            var task = queue.Enqueue(item0);
            Test.Async(async () =>
            {
                var result0 = await queue.Dequeue();
                Assert.AreSame(item0, result0);
            });
        }

        [TestMethod]
        public void Dequeue_WithoutResult_CompletesWithDefaultResult()
        {
            var queue = new WaitQueue<ItemNode<int>>() as IWaitQueue<ItemNode<int>>;
            var task0 = queue.Enqueue(item0);
            var result = queue.Dequeue().Result;
            task0.Wait();
            Assert.AreEqual<ItemNode<int>>(item0, result);
        }

        [TestMethod]
        public void Dequeue_EndEnqueue_IsComplete()
        {
            var queue = new WaitQueue<ItemNode<int>>() as IWaitQueue<ItemNode<int>>;
            var task1 = queue.Enqueue(item0);
            var task2 = queue.Enqueue(item1);

            Test.Async(async () =>
            {
                await task1;
                await task2;
                Assert.IsFalse(await queue.IsEmpty());
                Assert.IsFalse(await queue.IsComplete());
                Assert.IsFalse(await queue.IsEnded());
                await queue.EndEnqueue();
                Assert.IsTrue(await queue.IsEnded());
                Assert.IsFalse(await queue.IsEmpty());
                Assert.IsFalse(await queue.IsComplete());
                while (queue.Count > 0)
                    await queue.Dequeue();
                Assert.IsTrue(await queue.IsComplete());
                Assert.IsTrue(await queue.IsEmpty());
                Assert.IsTrue(await queue.IsEnded());
            });

            Assert.IsTrue(task1.IsCompleted);
            Assert.IsTrue(task2.IsCompleted);
        }

        [TestMethod]
        public void Enqueue_Cancel()
        {
            var wq = new WaitQueue<ItemNode<int>>();
            var queue = wq as IWaitQueue<ItemNode<int>>;
            var cts = new CancellationTokenSource();
            Test.Async(async () =>
            {
                // gets internal releaser for wait queue
                var releaser = (IDisposable) await wq.GetAwait();

                var task = queue.Enqueue(item0, cts.Token); // will block waiting for releaser
                Assert.IsFalse(task.IsCompleted);

                cts.Cancel();  // now cancel enqueue while it's blocked
                await AssertEx.ThrowsExceptionAsync<OperationCanceledException>(task);

                // clear the internal lock
                releaser.Dispose();

                Assert.IsTrue(await queue.IsEmpty());
                Assert.IsTrue(task.IsCanceled);
            });
        }

        [TestMethod]
        public void Cancelled_Dequeue()
        {
            var queue = new WaitQueue<ItemNode<int>>() as IWaitQueue<ItemNode<int>>;
            var cts = new CancellationTokenSource();
            var task = queue.Dequeue(cts.Token);

            Test.Async(async () =>
            {
                await Task.Delay(10);
                Assert.AreEqual<int>(1, queue.WaitCount);
                cts.Cancel();
                await AssertEx.ThrowsExceptionAsync<OperationCanceledException>(task);
                Assert.IsTrue(await queue.IsEmpty());
            });
        }

        [TestMethod]
        public void Cancelled_BeforeEnqueue_SynchronouslyCancelsTask()
        {
            var queue = new WaitQueue<ItemNode<int>>() as IWaitQueue<ItemNode<int>>;
            var cts = new CancellationTokenSource();
            cts.Cancel();
            var task = queue.Enqueue(item0, cts.Token);
            Assert.IsTrue(task.IsCanceled);
            Assert.IsTrue(queue.IsEmpty().Result);
        }

        [TestMethod]
        public void Cancelled_BeforeEnqueue_RemovesTaskFromQueue()
        {
            var queue = new WaitQueue<ItemNode<int>>() as IWaitQueue<ItemNode<int>>;
            var cts = new CancellationTokenSource();
            cts.Cancel();
            var task = queue.Dequeue(cts.Token);
            Assert.IsTrue(task.IsCanceled);
            Assert.IsTrue(queue.IsEmpty().Result);
        }

    }
}
