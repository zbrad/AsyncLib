using System;
using C = System.Collections;
using G = System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ZBrad.AsyncLib.Nodes;
using ZBrad.AsyncLib.Links;

namespace ZBrad.AsyncLib.Collections
{
    internal class List<T> : LinkedList<Node<T>>
    {

    }

    internal class LinkedList<N> : NodeCollection<N> where N : ILink
    {
        public N Head { get { return this.Root; } }
        public N Tail { get { return (N) this.Root?.Prev; } }

        public override N Root { get; protected set; }

        public virtual void InsertAtTail(N value)
        {
            if (WasFirst(value))
                return;

            InsertAfter(this.Tail, value);
        }

        public virtual void InsertAtHead(N value)
        {
            if (WasFirst(value))
                return;

            InsertBefore(this.Head, value);
        }

        protected bool WasFirst(N value)
        {
            if (this.Count != 0)
                return false;

            value.Prev = value.Next = value;
            this.Root = value;
            IncrementCount();
            return true;
        }

        protected void InsertBefore(N cur, N value)
        {
            var prev = cur.Prev;
            value.Prev = prev;
            value.Next = cur;
            cur.Prev = value;
            prev.Next = value;

            if ((ILink) cur == (ILink) this.Root)
                this.Root = value;
            
            IncrementCount();
        }

        protected void InsertAfter(N cur, N value)
        {
            var next = cur.Next;
            value.Prev = cur;
            value.Next = next;
            cur.Next = value;
            next.Prev = value;

            IncrementCount();
        }

        public virtual N RemoveFromHead()
        {
            if (this.Count == 0)
                return default(N);

            var node = this.Head;
            unlink(node);
            return node;
        }

        public virtual N RemoveFromTail()
        {
            if (this.Count == 0)
                return default(N);

            var node = this.Tail;
            unlink(node);
            return node;
        }

        public override void Add(N item)
        {
            InsertAtTail(item);
        }

        public override bool Contains(N value)
        {
            if (this.Root == null)
                return false;

            INode<N> node;
            return tryFind(value, out node);
        }

        public override bool Remove(N value)
        {
            if (this.Count == 0)
                return false;

            INode<N> node;
            if (!tryFind(value, out node))
                return false;

            unlink(node);
            return true;
        }

        #region private methods

        void unlink(N a)
        {
            if (a.Next == (ILink) a)
            {
                // removing last item
                this.Root = default(N);
            }
            else
            {
                // removing from inner link
                a.Next.Prev = a.Prev;
                a.Prev.Next = a.Next;

                if ((ILink) a == (ILink) this.Root)
                    this.Root = (N)a.Next;
            }

            // clear links and update count
            a.Prev = a.Next = null;
            DecrementCount();
        }

        bool tryFind(N value, out INode<N> cur)
        {
            cur = this.Head;

            // if head element
            if (value.Equals(cur.Value))
                return true;

            // if it was the only element, then don't bother searching
            if (this.Count == 1)
                return false;

            // search from tail
            cur = this.Tail;
            while (cur != this.Head)
            {
                if (value.Equals(cur.Value))
                    return true;

                cur = cur.Prev;
            }

            cur = null;
            return false;
        }

        #endregion
    }
}
