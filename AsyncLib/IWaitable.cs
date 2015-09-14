using System.Threading.Tasks;

namespace ZBrad.AsyncLib
{
    public interface IWaitable
    {
        bool IsEnded { get; }
        Task<bool> IsEmptyAsync();
        Task<bool> IsCompleteAsync();

        void Completed();
    }
}