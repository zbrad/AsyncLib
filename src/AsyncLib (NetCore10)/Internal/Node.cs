using System;
using System.Threading;

namespace ZBrad.AsyncLib.Collections
{
    internal abstract class Node<T> : ILink, IEquatable<Node<T>> where T : IEquatable<T>
    {
        static long seq = 0;

        public Node() : this(default(T)) { }

        public Node(T item)
        {
            this.Item = item;
            id = Interlocked.Increment(ref seq);
        }

        public ILink Prev { get; set; }
        public ILink Next { get; set; }

        public T Item { get; set; }

        long id = 0;
        public long Id { get { return id; } }

        public override string ToString()
        {
            return "{ id=" + id + ", value=" + this.Item.ToString() + "}";
        }

        public bool Equals(Node<T> other)
        {
            if (other == null)
                return false;

            return this.Item.Equals(other.Item);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Node<T>);
        }

        public override int GetHashCode()
        {
            return this.Item.GetHashCode();
        }
    }
}
