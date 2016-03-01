using System;
using C = System.Collections;
using G = System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ZBrad.AsyncLib
{
    internal class NodeList<N> : NodeCollection<N>, IList<N> where N : INode, IEquatable<N>
    {
        public virtual bool InsertAtTail(N node)
        {
            return InsertAfter((N) this.Tail, node);
        }

        public virtual bool InsertAtHead(N node)
        {
            return InsertBefore((N) this.Head, node);
        }

        public bool InsertBefore(N cur, N node)
        { 
            if (this.Count == 0)
            {
                node.Prev = node.Next = node;
                this.Head = this.Tail = node;
                IncrementCount();
                return true;
            }

            if (cur == null)
                return false;

            var prev = cur.Prev;
            node.Prev = prev;
            node.Next = cur;
            cur.Prev = node;
            prev.Next = node;

            if ((INode)cur == this.Head)
                this.Head = node;
            
            IncrementCount();
            return true;
        }

        public bool InsertAfter(N cur, N node)
        { 

            if (this.Count == 0)
            {
                node.Prev = node.Next = node;
                this.Head = this.Tail = node;
                IncrementCount();
                return true;
            }

            if (cur == null)
                return false;

            var next = cur.Next;
            node.Prev = cur;
            node.Next = next;
            cur.Next = node;
            next.Prev = node;

            if ((INode)cur == Tail)
                Tail = node;

            IncrementCount();
            return true;
        }

        public override bool Remove(N node)
        {
            if (node == null || this.Count == 0)
                return false;

            if (node.Prev == null && node.Next == null)
                return false;

            Unlink(node);
            return true;
        }

        public virtual N RemoveFromHead()
        {
            if (this.Count == 0)
                return default(N);

            var node = (N) this.Head;
            Unlink(node);
            return node;
        }

        public virtual N RemoveFromTail()
        {
            if (this.Count == 0)
                return default(N);

            var node = (N) this.Tail;
            Unlink(node);
            return node;
        }

        void Unlink(INode a)
        {
            if (a.Next == a)
            {
                // removing last item
                this.Head = this.Tail = null;
            }
            else
            {
                // removing from inner link
                a.Next.Prev = a.Prev;
                a.Prev.Next = a.Next;

                if (Head == a)
                    Head = (N) a.Next;
                if (Tail == a)
                    Tail = (N) a.Prev;
            }

            // clear links and update count
            a.Prev = a.Next = null;
            DecrementCount();
        }

        public override void Add(N item)
        {
            InsertAtTail(item);
        }

        public override bool Contains(N item)
        {
            if (this.Root == null)
                return false;

            INode cur = this.Root;
            
            while (!item.Equals((N)cur))
            {
                cur = cur.Next;

                // did we wrap?
                if (cur == this.Root)
                    return false;
            }

            return true;
        }
    }
}
