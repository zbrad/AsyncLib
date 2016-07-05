using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ZBrad.AsyncLib.Collections
{
    internal class ListAsync<T> : IAsyncList<T>
    {
        long version = 0;

        public AwaitLock Lock { get; }

        List<T> list = new List<T>();

        public IList<T> List { get { return list; } }

        public int Count { get { return list.Count; } }

        public async Task<T> Get(int index, CancellationToken token)
        {
            using (await this.Lock.Wait(token))
            {
                return list[index];
            }
        }

        public async Task Set(int index, T value, CancellationToken token)
        {
            using (await this.Lock.Wait(token))
            {
                list[index] = value;
            }
        }

        public async Task<int> IndexOf(T item, CancellationToken token)
        {
            using (await this.Lock.Wait(token))
            {
                return list.IndexOf(item);
            }
        }

        public async Task Insert(int index, T item, CancellationToken token)
        {
            using (await this.Lock.Wait(token))
            {
                list.Insert(index, item);
                version++;
            }
        }

        public async Task RemoveAt(int index, CancellationToken token)
        {
            using (await this.Lock.Wait(token))
            {
                list.RemoveAt(index);
                version++;
            }
        }

        public async Task Add(T item, CancellationToken token)
        {
            using (await this.Lock.Wait(token))
            {
                list.Add(item);
                version++;
            }
        }

        public async Task<bool> Remove(T item, CancellationToken token)
        {
            using (await this.Lock.Wait(token))
            {
                version++;
                return list.Remove(item);
            }
        }

        public async Task Clear(CancellationToken token)
        {
            using (await this.Lock.Wait(token))
            {
                list.Clear();
                version++;
            }
        }

        public async Task<bool> Contains(T item, CancellationToken token)
        {
            using (await this.Lock.Wait(token))
            {
                return list.Contains(item);
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
            ListAsync<T> list;
            IEnumerator<T> e;
            long version;

            public Enumerator(ListAsync<T> list)
            {
                this.list = list;
            }

            public T Current { get { return e.Current; } }

            public async Task<bool> MoveNext(CancellationToken token)
            {
                using (await this.list.Lock.Wait(token))
                {
                    if (e == null)
                    {
                        e = list.List.GetEnumerator();
                        version = list.version;
                    }

                    if (version != list.version)
                        throw new InvalidOperationException("list modified");

                    return e.MoveNext();
                }
            }

            public Task Reset(CancellationToken token)
            {
                e = null;
                return TaskEx.True;
            }

            public void Dispose()
            {
                if (e != null)
                    e.Dispose();

                e = null;
                list = null;
            }
        }
    }
}
