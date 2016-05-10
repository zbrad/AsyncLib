using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ZBrad.AsyncLib
{
    public abstract class Value<T> : Node<T>, IEquatable<Value<T>>, IComparable<Value<T>> where T : IEquatable<T>,IComparable<T>
    {
        public Value() : this(default(T)) { }

        public Value(T item) : base(item) { }

        public int CompareTo(Value<T> other)
        {
            return this.Item.CompareTo(other.Item);
        }

        public bool Equals(Value<T> other)
        {
            return base.Equals((Node<T>)other);
        }
    }

    public abstract class Node<T> : ILink, IEquatable<Node<T>> where T : IEquatable<T>
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
