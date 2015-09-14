using System;
using C = System.Collections;
using G = System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ZBrad.AsyncLib
{
    internal class NodeList<N> : NodeCollection<N>, INodeList<N> where N : class, INode
    {
        public virtual void InsertAtTail(N node)
        {
            InsertAfter(this.Tail, node);
        }

        public virtual void InsertAtHead(N node)
        {
            InsertBefore(this.Head, node);
        }

        public virtual void InsertBefore(N before, N newnode)
        {
            if (before == null || newnode == null)
                return;

            if (newnode.Prev != null || newnode.Next != null)
                return;
            
            InsertBefore((INode) before, (INode) newnode);
        }

        void InsertBefore(INode cur, INode node)
        { 
            if (this.Count == 0)
            {
                this.Head = this.Tail = node.Prev = node.Next = node;
                IncrementCount();
                return;
            }

            if (cur == null)
                return;

            var prev = cur.Prev;
            node.Prev = prev;
            node.Next = cur;
            cur.Prev = node;
            prev.Next = node;

            if (cur == this.Head)
                this.Head = node;
            
            IncrementCount();
        }

        public virtual void InsertAfter(N after, N newnode)
        {
            if (after == null || newnode == null)
                return;

            if (newnode.Prev != null || newnode.Next != null)
                return;

            InsertAfter((INode) after, (INode) newnode);
        }

        void InsertAfter(INode cur, INode node)
        { 

            if (this.Count == 0)
            {
                this.Head = this.Tail = node.Prev = node.Next = node;
                IncrementCount();
                return;
            }

            if (cur == null)
                return;

            var next = cur.Next;
            node.Prev = cur;
            node.Next = next;
            cur.Next = node;
            next.Prev = node;

            if (cur == Tail)
                Tail = node;

            IncrementCount();
        }
    }
}
