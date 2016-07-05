using System;
using System.Threading;
using System.Threading.Tasks;

namespace ZBrad.AsyncLib.Collections
{
    internal interface IAsyncLinkQueue<T> : IAsyncCollection<T> where T : ILink, IEquatable<T>
    {
        ILinkQueue<T> Queue { get; }
        Task<T> Dequeue(CancellationToken token);
        Task Enqueue(T item, CancellationToken token);
        Task<T> Peek(CancellationToken token);
        Task TrimExcess(CancellationToken token);
        Task<ITry<T>> TryFromHead(T value, CancellationToken token);
        Task<ITry<T>> TryFromTail(T value, CancellationToken token);
    }
}