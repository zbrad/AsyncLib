using System.Threading;
using System.Threading.Tasks;

namespace ZBrad.AsyncLib.Collections
{
    internal interface IAsyncQueue<T> : IAsyncCollection<T>
    {
        IQueue<T> Queue { get; }
        Task<T> Dequeue(CancellationToken token);
        Task Enqueue(T item, CancellationToken token);
        Task<T> Peek(CancellationToken token);
        Task TrimExcess(CancellationToken token);
    }
}