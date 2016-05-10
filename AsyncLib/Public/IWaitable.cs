using System.Threading.Tasks;
using System.Threading;

namespace ZBrad.AsyncLib
{
    public interface IWaitable
    {
        Task<bool> IsEnded(CancellationToken token);
        Task<bool> IsEmpty(CancellationToken token);
        Task<bool> IsComplete(CancellationToken token);
        Task EndEnqueue(CancellationToken token);
    }
}