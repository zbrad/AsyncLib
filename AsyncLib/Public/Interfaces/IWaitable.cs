using System.Threading.Tasks;
using System.Threading;

namespace ZBrad.AsyncLib
{
    public interface IWaitable
    {
        Task<bool> IsEndedAsync();
        Task<bool> IsEndedAsync(CancellationToken token);
        Task<bool> IsEmptyAsync();
        Task<bool> IsEmptyAsync(CancellationToken token);
        Task<bool> IsCompleteAsync();
        Task<bool> IsCompleteAsync(CancellationToken token);

        void EndEnqueue();
    }
}