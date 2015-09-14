using G = System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Threading;
namespace ZBrad.AsyncLib
{
    public interface INodeQueue<N> : INodeCollection<N> where N : INode
    {
        void Enqueue(N item);
        N Dequeue();
        N PeekHead();
        N PeekTail();
    }

    public interface INodeQueueOrdered<N> : INodeCollection<N> where N : INodeComparable<N>, IComparable<N>
    {
        void Enqueue(N item);
        N Dequeue();
        N PeekHead();
        N PeekTail();
    }
    public interface INodeQueueAsync<N> : INodeCollectionAsync<N> where N : INode
    {
        Task Enqueue(N item);
        Task Enqueue(N item, CancellationToken token);

        Task<N> Dequeue();
        Task<N> Dequeue(CancellationToken token);

        N PeekHead();
        N PeekTail();
    }

    public interface INodeList<N> : INodeCollection<N> where N : INode
    {
        INode Head { get; }
        INode Tail { get; }
        void InsertAtHead(N node);
        void InsertAtTail(N node);
        N RemoveFromHead();
        N RemoveFromTail();
        void Remove(N node);
        void InsertBefore(N cur, N node);
        void InsertAfter(N cur, N node);
    }

    public interface INodeListAsync<N> : INodeCollectionAsync<N> where N : INode
    {
        INode Head { get; }
        INode Tail { get; }
        Task InsertAtHead(N node);
        Task InsertAtTail(N node);
        Task<N> RemoveFromHead();
        Task<N> RemoveFromTail();
        Task Remove(N node);
        Task InsertBefore(N cur, N node);
        Task InsertAfter(N cur, N node);

        // cancellable versions
        Task InsertAtHead(N node, CancellationToken token);
        Task InsertAtTail(N node, CancellationToken token);

        Task<N> RemoveFromHead(CancellationToken token);
        Task<N> RemoveFromTail(CancellationToken token);
        Task Remove(N node, CancellationToken token);
        Task InsertBefore(N cur, N node, CancellationToken token);
        Task InsertAfter(N cur, N node, CancellationToken token);
    }

    public interface INodeListOrdered<N> : INodeCollection<N> where N : INodeComparable<N>, IComparable<N>
    {
        INode Head { get; }
        INode Tail { get; }
        void InsertFromHead(N node);
        void InsertFromTail(N node);
        void InsertFrom(N cur, N node);
        N RemoveFromHead();
        N RemoveFromTail();
        void Remove(N node);
    }



    public interface IOrderedListAsync<N> : INodeCollectionAsync<N> where N : INodeComparable<N>,IComparable<N>
    {
        INode Head { get; }
        INode Tail { get; }
        Task InsertFromHead(N node);
        Task InsertFromTail(N node);
        Task InsertFrom(N cur, N node);
        Task<N> RemoveFromHead();
        Task<N> RemoveFromTail();
        Task Remove(N node);

        // cancellable versions
        Task InsertFromHead(N node, CancellationToken token);
        Task InsertFromTail(N node, CancellationToken token);
        Task InsertFrom(N cur, N node, CancellationToken token);

        Task<N> RemoveFromHead(CancellationToken token);
        Task<N> RemoveFromTail(CancellationToken token);
        Task Remove(N node, CancellationToken token);
    }
}