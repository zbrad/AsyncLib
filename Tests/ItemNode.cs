using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZBrad.AsyncLib;
using System.Threading;

namespace Tests
{
    // IValue<T>, IEquatable<ItemNode<T>>, IComparable<ItemNode<T>>, INodeComparable<ItemNode<T>>
    internal class ItemNode<T> : IOrdered<T>, INodeComparable<ItemNode<T>>, IEquatable<ItemNode<T>> where T : IComparable<T>, IEquatable<T>
    {
        static int sequence;
        public int Id = Interlocked.Increment(ref sequence);
        public T Item { get; set; }
        public INode Next { get; set; }
        public INode Prev { get; set; }

        public int CompareTo(INodeComparable<T> other)
        {
            if (other == null)
                return -1;

            return CompareTo(other as ItemNode<T>);
        }

        public int CompareTo(INodeComparable<IOrdered<T>> other)
        {
            if (other == null)
                return -1;

            return CompareTo(other as ItemNode<T>);
        }

        public int CompareTo(IValue<T> other)
        {
            if (other == null)
                return -1;

            return CompareTo(other as ItemNode<T>);
        }

        public int CompareTo(INodeComparable<ItemNode<T>> other)
        {
            if (other == null)
                return -1;

            return CompareTo(other as ItemNode<T>);
        }

        public int CompareTo(ItemNode<T> other)
        {
            if (other == null)
                return -1;

            return Item.CompareTo(other.Item);
        }

        public bool Equals(IValue<T> other)
        {
            return Item.Equals(other.Item);
        }

        public bool Equals(IOrdered<T> other)
        {
            return Equals(other as IValue<T>);
        }

        public bool Equals(ItemNode<T> other)
        {
            if (other == null)
                return false;

            return Equals(other as IValue<T>);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IValue<T>);
        }

        public override int GetHashCode()
        {
            return Item.GetHashCode();
        }

        public override string ToString()
        {
            return "{ Value=" + Item + " Id=" + Id + " }";
        }
    }

}
