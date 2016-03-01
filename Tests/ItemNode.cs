using S = System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZBrad.AsyncLib;
using System.Threading;

namespace Tests
{
    internal class ItemNode<T> : IOrdered<T>, S.IEquatable<ItemNode<T>>, S.IComparable<ItemNode<T>> where T : S.IComparable<T>, S.IEquatable<T>
    {
        static int sequence;
        public int Id = Interlocked.Increment(ref sequence);
        public T Item { get; set; }
        public INode Next { get; set; }
        public INode Prev { get; set; }

        public int CompareTo(ItemNode<T> other)
        {
            throw new S.NotImplementedException();
        }

        public int CompareTo(IOrdered<T> other)
        {
            return this.Item.CompareTo(other.Item);
        }

        public bool Equals(ItemNode<T> other)
        {
            throw new S.NotImplementedException();
        }

        public bool Equals(IOrdered<T> other)
        {
            return this.Item.Equals(other.Item);
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
