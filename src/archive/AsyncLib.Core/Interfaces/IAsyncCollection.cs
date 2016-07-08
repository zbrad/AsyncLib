using System;
using System.Threading;
using System.Threading.Tasks;

namespace ZBrad.AsyncLib.Collections
{
    public interface IAsyncEnumerable<T>
    {
        IAsyncEnumerator<T> GetAsyncEnumerator();
    }

    public interface IAsyncEnumerator<T> : IDisposable
    {
        T Current { get; }
        Task Reset(CancellationToken token);
        Task<bool> MoveNext(CancellationToken token);
    }

    public interface IAsyncCollection<T> : IAsyncEnumerable<T>
    {
        AwaitLock Lock { get; }
        int Count { get; }
        Task CopyTo(T[] array, int arrayIndex, CancellationToken token);
    }
}
