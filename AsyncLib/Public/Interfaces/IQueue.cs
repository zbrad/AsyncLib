using G = System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Threading;
namespace ZBrad.AsyncLib
{
    public interface IQueue<N> : INodeCollection<N> where N : INode
    {
        void Enqueue(N item);
        N Dequeue();
        N PeekHead();
        N PeekTail();
    }

    public interface INodeQueueOrdered<N> : INodeCollection<N> where N : INodeComparable<N>, IComparable<N>, IEquatable<N>
    {
        void Enqueue(N item);
        N Dequeue();
        N PeekHead();
        N PeekTail();
    }
    public interface IQueueAsync<N> : INodeCollectionAsync<N> where N : INode
    {
        Task EnqueueAsync(N item);
        Task EnqueueAsync(N item, CancellationToken token);

        Task<N> DequeueAsync();
        Task<N> DequeueAsync(CancellationToken token);

        Task<N> PeekHeadAsync();
        Task<N> PeekTailAsync();
    }

}