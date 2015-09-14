using System.Collections.Generic;
using System;

namespace ZBrad.AsyncLib
{
    public interface IValue<T> : INode where T : IEquatable<T>
    {
        T Item { get; set; }
    }

    public interface IOrdered<T> : IComparable<IValue<T>>, IEquatable<IValue<T>>, IValue<T>, INodeComparable<T>, INodeComparable<IOrdered<T>> where T : IComparable<T>, IEquatable<T>
    {

    }
}