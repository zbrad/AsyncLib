using System.Threading;
using System.Threading.Tasks;
using G = System.Collections.Generic;
using System;

namespace ZBrad.AsyncLib.Links
{
    public interface ILink : IEquatable<ILink>
    {
        ILink Prev { get; set; }
        ILink Next { get; set; }
    }

    public interface ILinkEnumerable : G.IEnumerable<ILink>
    {

    }

    public interface ILinkAsyncEnumerable : ILinkEnumerable
    {
        ILinkAsyncEnumerator GetAsyncEnumerator();
    }

    public interface ILinkAsyncEnumerator : G.IEnumerator<ILink>
    {
        Task<bool> MoveNextAsync();
        Task<bool> MoveNextAsync(CancellationToken token);
    }

    public interface ILinkCollection : ILinkEnumerable, G.ICollection<ILink>
    {
        long Version { get; }
        ILink Root { get; }
    }

    public interface ILinkCollectionAsync : ILinkAsyncEnumerable, ILinkCollection
    {
        AwaitLock Locker { get; }

        Task<bool> AddAsync(ILink item);
        Task<bool> AddAsync(ILink item, CancellationToken token);

        Task<bool> RemoveAsync(ILink item);
        Task<bool> RemoveAsync(ILink item, CancellationToken token);

        Task<bool> ClearAsync();
        Task<bool> ClearAsync(CancellationToken token);

        Task<bool> ContainsAsync(ILink item);
        Task<bool> ContainsAsync(ILink item, CancellationToken token);

        Task<bool> CopyToAsync(ILink[] array, int arrayIndex);
        Task<bool> CopyToAsync(ILink[] array, int arrayIndex, CancellationToken token);
    }
}
