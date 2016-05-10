//using System;
//using C = System.Collections;
//using G = System.Collections.Generic;
//using System.Threading;

//namespace ZBrad.AsyncLib.Collections
//{
//    internal class Counter
//    {
//        public int Count { get; private set; }

//        public long Version { get; private set; }

//        public void Increment()
//        {
//            this.Count++;
//            this.Version++;
//        }

//        public void Decrement()
//        {
//            this.Count--;
//            this.Version++;
//        }

//        public void Reset()
//        {
//            this.Count = 0;
//            this.Version++;
//        }
//    }

//    internal abstract class Collection<T> : G.ICollection<T> where T : IEquatable<T>
//    {
//        protected Counter Counter { get; private set; } = new Counter();

//        public ILink Root { get; protected set; }

//        public long Version => this.Counter.Version;

//        public int Count => this.Counter.Count;

//        public bool IsReadOnly => false;

//        public abstract void Add(T item);

//        public abstract void CopyTo(T[] array, int arrayIndex);

//        public abstract void Clear();

//        public abstract bool Contains(T value);

//        public abstract bool Remove(T value);

//        public abstract G.IEnumerator<T> GetEnumerator();

//        C.IEnumerator C.IEnumerable.GetEnumerator()
//        {
//            return this.GetEnumerator();
//        }
//    }

//    internal abstract class LinkCollection<T> : ILinks<T> where T : ILink, IEquatable<T>
//    {
//        protected Counter Counter { get; private set; } = new Counter();

//        public ILink Root { get; protected set; }

//        public long Version => this.Counter.Version;

//        public int Count => this.Counter.Count;

//        public bool IsReadOnly => false;

//        public virtual void CopyTo(T[] array, int arrayIndex)
//        {
//            int i = 0;
//            var cur = this.Root;
//            do
//            {
//                array[arrayIndex + i] = (T)cur;
//                cur = cur.Next;
//                i++;
//            } while (cur != Root);
//        }

//        public virtual void Clear()
//        {
//            var cur = this.Root;
//            this.Counter.Reset();

//            while (cur != null)
//            {
//                var next = cur.Next;
//                cur.Next = cur.Prev = null;
//                cur = (T) next;
//            }
//        }

//        public ILinkEnumerator<T> GetEnumerator()
//        {
//            return new LinkEnumerator<T>(this);
//        }

//        G.IEnumerator<T> G.IEnumerable<T>.GetEnumerator()
//        {
//            return this.GetEnumerator();
//        }

//        C.IEnumerator C.IEnumerable.GetEnumerator()
//        {
//            return this.GetEnumerator();
//        }

//        public virtual void Add(T item)
//        {
//            if (InsertWasFirst(item))
//                return;

//            InsertAfter(this.Root.Prev, item);
//        }

//        public virtual bool Contains(T value)
//        {
//            if (this.Root == null)
//                return false;

//            ILink item;
//            return TryNext(value, this.Root, out item);
//        }

//        public virtual bool Remove(T value)
//        {
//            return Unlink(value);
//        }

//        public virtual bool TryNext(T value, T from, out T item)
//        {
//            ILink temp;
//            var result = this.TryNext(value, from, out temp);
//            item = (T)temp;
//            return result;
//        }

//        public virtual bool TryPrev(T value, T from, out T item)
//        {
//            ILink temp;
//            var result = this.TryPrev(value, from, out temp);
//            item = (T)temp;
//            return result;
//        }

//        protected bool InsertWasFirst(T node)
//        {
//            if (this.Count != 0)
//                return false;

//            node.Prev = node.Next = node;
//            this.Root = node;
//            this.Counter.Increment();
//            return true;
//        }

//        protected void InsertAfter(ILink cur, ILink value)
//        {
//            var next = cur.Next;
//            value.Prev = cur;
//            value.Next = next;
//            cur.Next = value;
//            next.Prev = value;

//            this.Counter.Increment();
//        }

//        protected void InsertBefore(ILink cur, ILink node)
//        {
//            var prev = cur.Prev;
//            node.Prev = prev;
//            node.Next = cur;
//            cur.Prev = node;
//            prev.Next = node;

//            if (cur == (ILink) this.Root)
//                this.Root = (T) node;

//            this.Counter.Increment();
//        }

//        protected bool Unlink(ILink a)
//        {
//            // if not linked, just return
//            if (a.Next == null || a.Prev == null)
//                return false;

//            if (a.Next == a)
//            {
//                // removing last item
//                this.Root = default(T);
//            }
//            else
//            {
//                // removing from inner link
//                a.Next.Prev = a.Prev;
//                a.Prev.Next = a.Next;

//                if (a == (ILink) this.Root)
//                    this.Root = (T) a.Next;
//            }

//            // clear links and update count
//            a.Prev = a.Next = null;
//            this.Counter.Decrement();
//            return true;
//        }

//        protected bool TryNext(ILink value, ILink from, out ILink cur)
//        {
//            cur = from;

//            // if root element
//            if (value.Equals(cur))
//                return true;

//            // if it was the only element, then don't bother searching
//            if (this.Count == 1)
//                return false;

//            // search to end
//            do
//            {
//                cur = cur.Next;
//                if (value.Equals(cur))
//                    return true;
//            }
//            while (cur != from);

//            // not found
//            return false;
//        }

//        protected bool TryPrev(ILink value, ILink from, out ILink cur)
//        {
//            cur = from;

//            // if from element
//            if (value.Equals(cur))
//                return true;

//            // if it was the only element, then don't bother searching
//            if (this.Count == 1)
//                return false;

//            // search to end
//            do
//            {
//                cur = cur.Prev;
//                if (value.Equals(cur))
//                    return true;
//            }
//            while (cur != from);

//            // not found
//            return false;
//        }
//    }
//}
