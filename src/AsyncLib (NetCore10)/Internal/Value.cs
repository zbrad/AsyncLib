using System;

namespace ZBrad.AsyncLib.Collections
{
    internal abstract class Value<T> : Node<T>, IEquatable<Value<T>>, IComparable<Value<T>> where T : IEquatable<T>, IComparable<T>
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
}
