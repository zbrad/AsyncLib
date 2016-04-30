using System;
using ZBrad.AsyncLib.Nodes;

namespace ZBrad.AsyncLib.Collections
{
    internal class Queue<T> : NodeCollection<T> where T : IEquatable<T>
    {
        LinkedList<T> nodes = new LinkedList<T>();

        public override Node<T> Root { get { return nodes.Root; } }

        public override int Count { get { return nodes.Count; } }

        public virtual void Enqueue(T node)
        {
            nodes.InsertAtTail(node);
        }

        public virtual T Dequeue()
        {
            return nodes.RemoveFromHead();
        }

        public virtual T PeekHead()
        {
            return nodes.Head.Value;
        }

        public virtual T PeekTail()
        {
            return nodes.Tail.Value;
        }

        public override void Add(T item)
        {
            this.Enqueue(item);
        }

        public override void Clear()
        {
            nodes.Clear();
        }

        public override bool Contains(T item)
        {
            return nodes.Contains(item);
        }

        public override bool Remove(T item)
        {
            return nodes.Remove(item);
        }
    }
}

