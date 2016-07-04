using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections;

namespace ZBrad.AsyncLib.Collections
{
    internal class LinkList<T> : ILinkList<T> where T : ILink, IEquatable<T>
    {
        ILink root;
        long version;
        int count;

        public int Count { get { return count; } }

        public T Head { get { return (T)root; } }

        public T Tail { get { return (T)root?.Prev; } }

        public virtual void InsertAtTail(T node)
        {
            if ((ILink)node == null)
                throw new ArgumentNullException("node");

            if (InsertWasFirst(node))
                return;

            InsertAfter(this.Tail, node);
        }

        public virtual void InsertAtHead(T node)
        {
            if ((ILink)node == null)
                throw new ArgumentNullException("node");

            if (InsertWasFirst(node))
                return;

            InsertBefore(this.Head, node);
        }

        public T RemoveFromHead()
        {
            if (this.Count == 0)
                return default(T);

            var node = this.Head;
            Unlink(node);
            return node;
        }

        public T RemoveFromTail()
        {
            if (this.Count == 0)
                return default(T);

            var node = this.Tail;
            Unlink(node);
            return node;
        }

        #region collection

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            int i = 0;
            var cur = root;
            do
            {
                array[arrayIndex + i] = (T)cur;
                cur = cur.Next;
                i++;
            } while (cur != root);
        }

        public void Clear()
        {
            var cur = root;
            count = 0;

            while (cur != null)
            {
                var next = cur.Next;
                cur.Next = cur.Prev = null;
                cur = next;
            }
        }

        public ILinkEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        public void Remove(ILink link)
        {
            if (link == null)
                throw new ArgumentNullException("link");

            Unlink(link);
        }

        /// <summary>
        /// search list for matching value using Next links (forward search)
        /// </summary>
        /// <param name="from"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ITry<T> TryNext(ILink from, T value)
        {
            if (from == null)
                throw new ArgumentNullException("from");
            if ((ILink)value == null)
                throw new ArgumentNullException("value");

            var p = from;

            // search to from (start) via next links
            do
            {
                var cur = (T)p; 
                if (value.Equals(cur))
                    return new Try<T>(cur);
                p = p.Next;
            }
            while (p != from);

            // not found
            return Try<T>.False;
        }

        /// <summary>
        /// search list for match value using Prev links (reverse search)
        /// </summary>
        /// <param name="from"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ITry<T> TryPrev(ILink from, T value)
        {
            if (from == null)
                throw new ArgumentNullException("from");
            if ((ILink)value == null)
                throw new ArgumentNullException("value");

            var p = from;

            // search to from (start) via prev links
            do
            {
                var cur = (T)p;
                if (value.Equals(cur))
                    return new Try<T>(cur);
                p = p.Prev;
            }
            while (p != from);

            // not found
            return Try<T>.False;
        }

        protected bool InsertWasFirst(T node)
        {
            if (this.Count != 0)
                return false;

            node.Prev = node.Next = node;
            root = node;
            count++;
            version++;
            return true;
        }

        public virtual void InsertAfter(ILink cur, T value)
        {
            if (cur == null)
                throw new ArgumentNullException("cur");

            if ((ILink)value == null)
                throw new ArgumentNullException("value");

            var next = cur.Next;
            value.Prev = cur;
            value.Next = next;
            cur.Next = value;
            next.Prev = value;

            count++;
            version++;
        }

        public virtual void InsertBefore(ILink cur, T value)
        {
            if (cur == null)
                throw new ArgumentNullException("cur");

            if ((ILink)value == null)
                throw new ArgumentNullException("value");

            var prev = cur.Prev;
            value.Prev = prev;
            value.Next = cur;
            cur.Prev = value;
            prev.Next = value;

            if (cur == root)
                root = value;

            count++;
            version++;
        }

        bool Unlink(ILink a)
        {
            // if not linked, just return
            if (a.Next == null || a.Prev == null)
                return false;

            if (a.Next == a)
            {
                // removing last item
                root = null;
            }
            else
            {
                // removing from inner link
                a.Next.Prev = a.Prev;
                a.Prev.Next = a.Next;

                if (a == root)
                    root = a.Next;
            }

            // clear links and update count
            a.Prev = a.Next = null;
            count--;
            version++;
            return true;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        class Enumerator : ILinkEnumerator<T>
        {
            ILink current;
            long version;
            LinkList<T> list;

            public Enumerator(LinkList<T> list)
            {
                this.list = list;
            }

            public T Current { get { return (T)current; } }

            object IEnumerator.Current { get { return this.Current; } }

            public void Dispose()
            {
                list = null;
            }

            public bool MoveNext()
            {
                if (current == null)
                {
                    current = list.root;
                    version = list.version;
                    return true;
                }

                if (version != list.version)
                    throw new InvalidOperationException("list modified");

                current = current.Next;
                return current != list.root;
            }

            public void Reset()
            {
                current = null;
            }
        }
    }
}
