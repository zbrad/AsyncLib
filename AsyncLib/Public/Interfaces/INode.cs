using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using G = System.Collections.Generic;
using C = System.Collections;

namespace ZBrad.AsyncLib
{
    public interface INode
    {
        INode Prev { get; set; }
        INode Next { get; set; }
    }

    public interface INodeComparable<N> : INode, IComparable<INodeComparable<N>> where N : IComparable<N>, IEquatable<N>
    {

    }

    public interface INodeEnumerable<N> : G.IEnumerable<N> where N : INode
    {

    }

    public interface INodeEnumerableAsync<N> where N : INode
    {
        INodeEnumeratorAsync<N> GetEnumeratorAsync();
    }

    public interface INodeEnumeratorAsync<N> where N : INode
    {
        N Current { get; }
        Task<bool> MoveNextAsync();
        Task ResetAsync();
    }

    public interface INodeCollection<N> : INodeEnumerable<N> where N : INode
    {
        int Version { get; }
        INode Root { get; }
        int Count { get; }
        void CopyTo(N[] array, int arrayIndex);
    }

    public interface INodeCollectionAsync<N> : INodeEnumerableAsync<N> where N : INode
    {
        int Version { get; }
        AwaitLock Locker { get; }
        Task<INode> GetRootAsync(int version);
        Task<int> CountAsync { get; }
        Task CopyToAsync(N[] array, int arrayIndex);
        Task CopyToAsync(N[] array, int arrayIndex, CancellationToken token);
    }
}
