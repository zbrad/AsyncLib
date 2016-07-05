using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ZBrad.AsyncLib.Collections
{
    internal class AsyncLinkList<T> : IAsyncLinkList<T> where T : ILink, IEquatable<T>
    {
        LinkList<T> list;

        public AsyncLinkList()
        {
            list = new LinkList<T>();
        }

        public ILink Root { get { return list.Head; } }

        public T Head { get { return list.Head; } }

        public T Tail { get { return list.Tail; } }

        public ILinkList<T> List { get { return list; } }

        public AwaitLock Lock { get; private set; } = new AwaitLock();

        public int Count { get { return list.Count; } }

        public Task InsertAtHead(T item, CancellationToken token)
        {
            return InsertBefore(this.Head, item, token);
        }

        public Task InsertAtTail(T item, CancellationToken token)
        {
            return InsertAfter(this.Tail, item, token);
        }

        public async Task<T> RemoveFromHead(CancellationToken token)
        {
            using (await this.Lock.Wait(token))
            {
                return list.RemoveFromHead();
            }
        }

        public async Task<T> RemoveFromTail(CancellationToken token)
        {
            using (await this.Lock.Wait(token))
            {
                return list.RemoveFromTail();
            }
        }

        public async Task InsertBefore(ILink link, T item, CancellationToken token)
        {
            using (await this.Lock.Wait(token))
            {
                list.InsertBefore(link, item);
            }
        }

        public async Task InsertAfter(ILink link, T item, CancellationToken token)
        {
            using (await this.Lock.Wait(token))
            {
                list.InsertAfter(link, item);
            }
        }

        public async Task Remove(T item, CancellationToken token)
        {
            using (await this.Lock.Wait(token))
            {
                list.Remove(item);
            }
        }

        public async Task<ITry<T>> TryNext(ILink from, T value, CancellationToken token)
        {
            using (await this.Lock.Wait(token))
            {
                return list.TryNext(from, value);
            }
        }

        public async Task<ITry<T>> TryPrev(ILink from, T value, CancellationToken token)
        {
            using (await this.Lock.Wait(token))
            {
                return list.TryPrev(from, value);
            }
        }

        public async Task CopyTo(T[] array, int arrayIndex, CancellationToken token)
        {
            using (await this.Lock.Wait(token))
            {
                list.CopyTo(array, arrayIndex);
            }
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator()
        {
            return new Enumerator(this);
        }

        class Enumerator : IAsyncEnumerator<T>
        {
            AsyncLinkList<T> list;
            IEnumerator<T> e;

            public Enumerator(AsyncLinkList<T> list)
            {
                this.list = list;
            }

            public T Current { get { return e.Current; } }

            public void Dispose()
            {
                if (e != null)
                    e.Dispose();

                e = null;
                list = null;
            }

            public async Task<bool> MoveNext(CancellationToken token)
            {
                using (await list.Lock.Wait(token))
                {
                    if (e == null)
                        e = list.List.GetEnumerator();

                    return e.MoveNext();
                }
            }

            public Task Reset(CancellationToken token)
            {
                e = null;
                return TaskEx.True;
            }
        }
    }
}
