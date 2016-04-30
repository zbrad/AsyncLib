using C = System.Collections;
using G = System.Collections.Generic;
using System;
using ZBrad.AsyncLib.Nodes;

namespace ZBrad.AsyncLib.Collections
{
    internal class OrderedQueue<T> : NodeCollection<INode<T>> where T : IComparable<T>
    {
        OrderedList<T> nodes = new OrderedList<T>();

        public override int Count { get { return nodes.Count; } }

        public virtual void Enqueue(T value)
        {
            var node = new Node<T>(value);
            nodes.InsertAtTail(node);
        }

        public virtual T Dequeue()
        {
            return nodes.RemoveFromHead();
        }

        public virtual T PeekHead()
        {
            if (nodes.Count == 0)
                return default(T);

            return nodes.Head.Value;
        }

        public virtual T PeekTail()
        {
            if (nodes.Count == 0)
                return default(T);

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
