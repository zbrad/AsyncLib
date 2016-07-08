using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ZBrad.AsyncLib.Collections
{
    public interface ILinkEnumerator<T> : IEnumerator<T> where T : ILink
    {

    }

    public interface ILinkList<T> : ICollectionEx<T> where T : ILink, IEquatable<T>
    {
        T Head { get; }
        T Tail { get; }

        void InsertAtHead(T item);
        void InsertAtTail(T item);

        void InsertBefore(ILink link, T item);
        void InsertAfter(ILink link, T item);
        void Remove(ILink link);

        T RemoveFromHead();
        T RemoveFromTail();

        ITry<T> TryNext(ILink from, T value);
        ITry<T> TryPrev(ILink from, T value);
    }

    public interface IAsyncLinkList<T> : IAsyncCollection<T> where T : ILink, IEquatable<T>
    {
        T Head { get; }
        T Tail { get; }

        ILinkList<T> List { get; }

        Task InsertAtHead(T item, CancellationToken token);
        Task InsertAtTail(T item, CancellationToken token);

        Task<T> RemoveFromHead(CancellationToken token);
        Task<T> RemoveFromTail(CancellationToken token);

        Task InsertBefore(ILink link, T item, CancellationToken token);
        Task InsertAfter(ILink link, T item, CancellationToken token);
        Task Remove(T item, CancellationToken token);

        Task<ITry<T>> TryNext(ILink from, T value, CancellationToken token);
        Task<ITry<T>> TryPrev(ILink from, T value, CancellationToken token);
    }
}