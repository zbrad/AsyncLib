using System.Threading;
using System.Threading.Tasks;
using G = System.Collections.Generic;
using System;
using ZBrad.AsyncLib.Links;

namespace ZBrad.AsyncLib.Nodes
{
    public interface INodeEnumerable<T> : G.IEnumerable<T>
    {

    }

    public interface INodeAsyncEnumerable<T>
    {
        INodeAsyncEnumerator<T> GetAsyncEnumerator();
    }

    public interface INodeAsyncEnumerator<T>
    {
        T Current { get; }
        void Reset();
        Task<bool> MoveNextAsync();
        Task<bool> MoveNextAsync(CancellationToken token);
    }

    public interface INodeCollection<T> : INodeEnumerable<T> where T : ILink
    {
        T Root { get; }
        long Version { get; }
        int Count { get; }
        bool IsReadOnly { get; }
        void Add(T item);
        void Clear();
        void CopyTo(T[] array, int arrayIndex);
        bool Contains(T node);
        bool Remove(T node);
    }

    public interface INodeAsyncCollection<T> : INodeAsyncEnumerable<T>
    {
        int Count { get; }
        bool IsReadOnly { get; }

        long Version { get; }
        INode<T> Root { get; }

        AwaitLock Locker { get; }

        Task<bool> AddAsync(T item);
        Task<bool> AddAsync(T item, CancellationToken token);

        Task<bool> RemoveAsync(T item);
        Task<bool> RemoveAsync(T item, CancellationToken token);

        Task ClearAsync();
        Task ClearAsync(CancellationToken token);

        Task<bool> ContainsAsync(T item);
        Task<bool> ContainsAsync(T item, CancellationToken token);

        Task CopyToAsync(T[] array, int arrayIndex);
        Task CopyToAsync(T[] array, int arrayIndex, CancellationToken token);
    }
}
