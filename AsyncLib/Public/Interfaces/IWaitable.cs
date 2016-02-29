using System.Threading.Tasks;

namespace ZBrad.AsyncLib
{
    public interface IWaitable
    {
        Task<bool> IsEndedAsync();
        Task<bool> IsEmptyAsync();
        Task<bool> IsCompleteAsync();

        void EndEnqueue();
    }
}