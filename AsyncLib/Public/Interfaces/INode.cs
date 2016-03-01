using System.Threading;
using System.Threading.Tasks;
using G = System.Collections.Generic;
using S = System;

namespace ZBrad.AsyncLib
{
    public interface INode
    {
        INode Prev { get; set; }
        INode Next { get; set; }
    }

    //public interface INodeComparable<N> : INode, S.IComparable<INodeComparable<N>> where N : S.IComparable<N>, S.IEquatable<N>
    //{

    //}

    public interface INodeEnumerable<N> : G.IEnumerable<N> where N : INode
    {

    }

    public interface IAsyncEnumerable<N> : INodeEnumerable<N> where N : INode
    {
        IAsyncEnumerator<N> GetAsyncEnumerator();
    }

    public interface IAsyncEnumerator<N> : G.IEnumerator<N> where N : INode
    {
        Task<bool> MoveNextAsync();
        Task<bool> MoveNextAsync(CancellationToken token);
    }

    public interface INodeCollection<N> : INodeEnumerable<N>, G.ICollection<N> where N : INode, S.IEquatable<N>
    {
        long Version { get; }
        INode Root { get; }
    }

    public interface INodeCollectionAsync<N> : IAsyncEnumerable<N>, INodeCollection<N> where N : INode, S.IEquatable<N>
    {
        AwaitLock Locker { get; }

        Task<bool> AddAsync(N item);
        Task<bool> AddAsync(N item, CancellationToken token);

        Task<bool> RemoveAsync(N item);
        Task<bool> RemoveAsync(N item, CancellationToken token);

        Task<bool> ClearAsync();
        Task<bool> ClearAsync(CancellationToken token);

        Task<bool> ContainsAsync(N item);
        Task<bool> ContainsAsync(N item, CancellationToken token);

        Task<bool> CopyToAsync(N[] array, int arrayIndex);
        Task<bool> CopyToAsync(N[] array, int arrayIndex, CancellationToken token);
    }
}
