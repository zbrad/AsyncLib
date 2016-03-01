using S = System;
using G = System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZBrad.AsyncLib
{
    internal interface IOrdered<T> : INode, S.IComparable<IOrdered<T>>, S.IEquatable<IOrdered<T>> where T : S.IComparable<T>, S.IEquatable<T>
    {
        T Item { get; set; }
    }

    internal class Ordered<T> : IOrdered<T> where T : S.IComparable<T>, S.IEquatable<T>
    {
        public T Item { get; set; }
        public INode Prev { get; set; }
        public INode Next { get; set; }
        public Ordered(T item)
        {
            this.Item = item;
        }

        public int CompareTo(IOrdered<T> other)
        {
            return this.Item.CompareTo(other.Item);
        }

        public bool Equals(IOrdered<T> other)
        {
            return this.Item.Equals(other.Item);
        }
    }
}
