using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZBrad.AsyncLib
{
    internal class Ordered<T> : IOrdered<T>, INodeComparable<Ordered<T>> where T : IComparable<T>, IEquatable<T>
    {
        public T Item { get; set; }
        public INode Prev { get; set; }
        public INode Next { get; set; }
        public Ordered(T item)
        {
            this.Item = item;
        }

        public int CompareTo(IValue<T> other)
        {
            if (other == null)
                return -1;

            return Item.CompareTo(other.Item);
        }

        public int CompareTo(INodeComparable<T> other)
        {
            if (other == null)
                return -1;

            return CompareTo(other as IValue<T>);
        }

        public int CompareTo(INodeComparable<IOrdered<T>> other)
        {
            if (other == null)
                return -1;

            return CompareTo(other as IValue<T>);
        }

        public int CompareTo(INodeComparable<Ordered<T>> other)
        {
            if (other == null)
                return -1;

            return CompareTo(other as IValue<T>);
        }

        public bool Equals(IValue<T> other)
        {
            if (other == null)
                return false;

            return Item.Equals(other.Item);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IValue<T>);
        }

        public override int GetHashCode()
        {
            return Item.GetHashCode();
        }
    }


}
