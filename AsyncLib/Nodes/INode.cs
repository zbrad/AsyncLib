using System;
using ZBrad.AsyncLib.Links;

namespace ZBrad.AsyncLib.Nodes
{
    public interface INode<T> : ILink, IEquatable<INode<T>>
    {
        long Id { get; }
        T Value { get; }
    }
}