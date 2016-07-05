using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ZBrad.AsyncLib.Collections
{
    public interface IAsyncList<T> : IAsyncCollection<T>
    {
        IList<T> List { get; }
        Task<T> Get(int index, CancellationToken token);
        Task Set(int index, T value, CancellationToken token);
        Task<int> IndexOf(T item, CancellationToken token);
        Task Insert(int index, T item, CancellationToken token);
        Task RemoveAt(int index, CancellationToken token);
    }
}
