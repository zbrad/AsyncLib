using S = System;
using System.Threading;
using System.Threading.Tasks;

namespace ZBrad.AsyncLib
{
    public interface IList<N> : INodeCollection<N> where N : INode, S.IEquatable<N>
    {
        INode Head { get; }
        INode Tail { get; }
        bool InsertAtHead(N item);
        bool InsertAtTail(N item);
        N RemoveFromHead();
        N RemoveFromTail();
        bool InsertBefore(N item, N node);
        bool InsertAfter(N item, N node);
    }

    public interface IListAsync<N> : INodeCollectionAsync<N> where N : INode, S.IEquatable<N>
    {
        Task<bool> InsertAtHeadAsync(N item);
        Task<bool> InsertAtHeadAsync(N item, CancellationToken token);

        Task<bool> InsertAtTailAsync(N item);
        Task<bool> InsertAtTailAsync(N item, CancellationToken token);

        Task<N> RemoveFromHeadAsync();
        Task<N> RemoveFromHeadAsync(CancellationToken token);

        Task<N> RemoveFromTailAsync();
        Task<N> RemoveFromTailAsync(CancellationToken token);

        Task<bool> InsertBeforeAsync(N item, N node);
        Task<bool> InsertBeforeAsync(N item, N node, CancellationToken token);

        Task<bool> InsertAfterAsync(N item, N node);
        Task<bool> InsertAfterAsync(N item, N node, CancellationToken token);
    }

    public interface INodeListOrdered<N> : INodeCollection<N> where N : INode, S.IEquatable<N>, S.IComparable<N>
    {
        INode Head { get; }
        INode Tail { get; }
        bool InsertFromHead(N item);
        bool InsertFromTail(N item);
        bool InsertFrom(N item, N node);
        N RemoveFromHead();
        N RemoveFromTail();
    }

    public interface IOrderedListAsync<N> : INodeCollectionAsync<N> where N : INode, S.IEquatable<N>, S.IComparable<N>
    {
        INode Head { get; }
        INode Tail { get; }
        Task InsertFromHeadAsync(N item);
        Task InsertFromTailAsync(N item);
        Task InsertFromAsync(N item, N node);
        Task<N> RemoveFromHeadAsync();
        Task<N> RemoveFromTailAsync();

        // cancellable versions
        Task InsertFromHeadAsync(N item, CancellationToken token);
        Task InsertFromTailAsync(N item, CancellationToken token);
        Task InsertFromAsync(N item, N node, CancellationToken token);

        Task<N> RemoveFromHeadAsync(CancellationToken token);
        Task<N> RemoveFromTailAsync(CancellationToken token);
    }

}
