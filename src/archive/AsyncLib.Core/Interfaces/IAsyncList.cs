using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

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
