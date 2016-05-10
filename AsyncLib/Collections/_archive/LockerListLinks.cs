//using System;
//using System.Threading.Tasks;
//using System.Threading;

//namespace ZBrad.AsyncLib.Collections
//{
//    internal class LockerListLinks<T> : LockerLinks<T>, IListAsync<T> where T : ILink, IEquatable<T>
//    {
//        ListLinks<T> list = new ListLinks<T>();

//        public IListLinks<T> List { get { return list; } }

//        public T Head { get { return list.Head; } }

//        public T Tail { get { return list.Tail; } }

//        public async Task InsertAtTail(T node, CancellationToken token)
//        {
//            using (await this.Lock.Wait(token))
//            {
//                list.InsertAtTail(node);
//            }
//        }

//        public async Task InsertAtHead(T node, CancellationToken token)
//        {
//            using (await this.Lock.Wait(token))
//            {
//                list.InsertAtHead(node);
//            }
//        }

//        public async Task<T> RemoveFromHead(CancellationToken token)
//        {
//            using (await this.Lock.Wait(token))
//            {
//                return list.RemoveFromHead();
//            }
//        }

//        public async Task<T> RemoveFromTail(CancellationToken token)
//        {
//            using (await this.Lock.Wait(token))
//            {
//                return list.RemoveFromTail();
//            }
//        }
//    }
//}
