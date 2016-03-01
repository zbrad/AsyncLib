using G = System.Collections.Generic;
using S = System;
using System.Threading.Tasks;
using System.Threading;
namespace ZBrad.AsyncLib
{
    public interface IQueue<N> : INodeCollection<N> where N : INode, S.IEquatable<N>
    {
        bool Enqueue(N item);
        N Dequeue();
        N PeekHead();
        N PeekTail();
    }

    public interface IQueueOrdered<N> : INodeCollection<N> where N : INode, S.IEquatable<N>, S.IComparable<N>
    {
        bool Enqueue(N item);
        N Dequeue();
        N PeekHead();
        N PeekTail();
    }
    public interface IQueueAsync<N> : INodeCollectionAsync<N> where N : INode, S.IEquatable<N>
    {
        Task<bool> EnqueueAsync(N item);
        Task<bool> EnqueueAsync(N item, CancellationToken token);

        Task<N> DequeueAsync();
        Task<N> DequeueAsync(CancellationToken token);

        Task<N> PeekHeadAsync();
        Task<N> PeekHeadAsync(CancellationToken token);

        Task<N> PeekTailAsync();
        Task<N> PeekTailAsync(CancellationToken token);
    }

}