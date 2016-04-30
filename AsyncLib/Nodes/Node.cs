using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZBrad.AsyncLib.Links;
using System.Threading;

namespace ZBrad.AsyncLib.Nodes
{
    internal interface IComparableNode<T> : INode<T>, IComparable<INode<T>> where T : IComparable<T>
    {

    }

    internal class ComparableNode<T> : Node<T>, IComparableNode<T> where T : IComparable<T>
    {
        public int CompareTo(INode<T> other)
        {
            return this.Value.CompareTo(other.Value);
        }
    }

    internal class Node<T> : INode<T>
    {
        static long NodeSeq = 0;

        public Node() : this(default(T)) { }

        public Node(T value)
        {
            this.Value = value;
            id = Interlocked.Increment(ref NodeSeq);
        }

        public ILink Prev { get; set; }

        public ILink Next { get; set; }

        public T Value { get; set; }

        long id = 0;
        public long Id { get { return id; } }

        public override string ToString()
        {
            return "{ id=" + id + ", value=" + this.Value.ToString() + "}";
        }

        public bool Equals(INode<T> other)
        {
            if (other == null)
                return false;

            return this.Value.Equals(other.Value);
        }

        public bool Equals(ILink other)
        {
            return Equals(other as INode<T>);
        }
    }
}
