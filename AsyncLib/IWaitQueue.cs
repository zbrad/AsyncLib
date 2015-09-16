using System.Threading;
using System.Threading.Tasks;

namespace ZBrad.AsyncLib
{
    internal interface IWaitQueue<N> where N : class, INode
    {
        int WaitCount { get; }
        int Count { get; }
        Task EndEnqueue();
        Task CopyTo(N[] array, int arrayIndex);
        Task CopyTo(N[] array, int arrayIndex, CancellationToken token);
        Task<N> Dequeue();
        Task<N> Dequeue(CancellationToken token);
        Task Enqueue(N item);
        Task Enqueue(N item, CancellationToken token);
        INodeEnumeratorAsync<N> GetAsyncEnumerator();
        Task<bool> IsComplete();
        Task<bool> IsEmpty();
        Task<bool> IsEnded();
        N PeekHead();
        N PeekTail();
    }
}