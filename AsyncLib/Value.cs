using System;

namespace ZBrad.AsyncLib
{
    internal class Value<T> : IValue<T> where T : IEquatable<T>
    {
        public T Item { get; set; }

        public INode Prev { get; set; }
        public INode Next { get; set; }

        public Value(T item)
        {
            this.Item = item;
        }
    }
}