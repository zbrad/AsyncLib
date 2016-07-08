//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Threading;

//namespace ZBrad.AsyncLib.Collections
//{
//    internal class AsyncLinkEnumerator<T> : IAsyncEnumerator<T> where T : ILink, IEquatable<T>
//    {
//        static Task<T> cancelFault = TaskEx.CancelFault<T>();

//        ILinksAsync<T> collection;
//        long version;
//        ILink root;
//        ILink cur;

//        public AsyncLinkEnumerator(ILinksAsync<T> collection)
//        {
//            this.collection = collection;
//            this.root = this.collection.Root;
//            this.version = this.collection.Version;
//            this.cur = null;
//        }

//        public T Current { get { return (T)cur; } }

//        public async Task<bool> MoveNext(CancellationToken token)
//        {
//            using (await this.collection.Lock.Wait(token))
//            {
//                if (version != this.collection.Version)
//                    throw new InvalidOperationException("collection modified");

//                if (root == null)
//                    return false;

//                if (cur == null)
//                {
//                    cur = root;
//                    return true;
//                }

//                cur = cur.Next;
//                return cur != root;
//            }
//        }

//        public async Task Reset(CancellationToken token)
//        {
//            using (await this.collection.Lock.Wait(token))
//            {
//                if (version != this.collection.Version)
//                    throw new InvalidOperationException("collection modified");

//                this.cur = null;
//            }
//        }
//    }

//}
