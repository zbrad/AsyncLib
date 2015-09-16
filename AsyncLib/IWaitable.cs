using System.Threading.Tasks;

namespace ZBrad.AsyncLib
{
    public interface IWaitable
    {
        Task<bool> IsEnded();
        Task<bool> IsEmpty();
        Task<bool> IsComplete();

        Task EndEnqueue();
    }
}