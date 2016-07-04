//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;
//using System.Threading;

//namespace ZBrad.AsyncLib.Collections
//{
//    public interface ILocker<T> where T : IEquatable<T>
//    {
//        AwaitLock Lock { get; }
//    }

//    public interface ILockerLinks<T> : ILinksAsync<T>, ILocker<T> where T : ILink, IEquatable<T>
//    {
//    }

//    internal class Locker<T> : ILocker<T>, ICollectionAsync<T> where T : IEquatable<T>
//    {

//    }

//    internal class LockerLinks<T> : LinkCollection<T>, ILinksAsync<T>, ILockerLinks<T> where T : ILink, IEquatable<T>
//    {
//        public AwaitLock Lock { get; private set; } = new AwaitLock();

//        public async Task Add(T item, CancellationToken token)
//        {
//            using (await this.Lock.Wait(token))
//            {
//                base.Add(item);
//            }
//        }

//        public async Task<bool> Remove(T item, CancellationToken token)
//        {
//            using (await this.Lock.Wait(token))
//            {
//                return base.Remove(item);
//            }
//        }

//        public async Task Clear(CancellationToken token)
//        {
//            using (await this.Lock.Wait(token))
//            {
//                base.Clear();
//            }
//        }

//        public async Task<bool> Contains(T item, CancellationToken token)
//        {
//            using (await this.Lock.Wait(token))
//            {
//                return base.Contains(item);
//            }
//        }

//        public async Task CopyTo(T[] array, int arrayIndex, CancellationToken token)
//        {
//            using (await this.Lock.Wait(token))
//            {
//                base.CopyTo(array, arrayIndex);
//            }
//        }

//        public async Task<ITry<T>> TryNext(T value, T from, CancellationToken token)
//        {
//            using (await this.Lock.Wait(token))
//            {
//                ILink temp;
//                if (base.TryNext(value, from, out temp))
//                    return new Try<T>((T)temp);
//                return Try<T>.False;
//            }
//        }

//        public async Task<ITry<T>> TryPrev(T value, T from, CancellationToken token)
//        {
//            using (await this.Lock.Wait(token))
//            {
//                ILink temp;
//                if (base.TryPrev(value, from, out temp))
//                    return new Try<T>((T)temp);
//                return Try<T>.False;
//            }
//        }

//        public IAsyncEnumerator<T> GetAsyncEnumerator()
//        {
//            return new AsyncLinkEnumerator<T>(this);
//        }
//    }

//}
