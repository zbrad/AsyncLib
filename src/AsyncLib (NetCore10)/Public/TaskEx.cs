using System.Threading.Tasks;

namespace ZBrad.AsyncLib
{
    public static class TaskEx
    {
        static TaskCanceledException cancelException = new TaskCanceledException();

        public static Task<bool> CancelFaultBool = CancelFault<bool>();
        public static Task<int> CancelFaultInt = CancelFault<int>();
        public static Task<bool> True = Task<bool>.FromResult<bool>(true);
        public static Task<bool> False = Task<bool>.FromResult<bool>(false);

        public static Task<bool> CanceledBool;
        public static Task Canceled;

        static TaskEx()
        {
            // cancelled tasks
            var tCancel = new TaskCompletionSource<bool>();
            tCancel.SetCanceled();
            CanceledBool = tCancel.Task;
            Canceled = tCancel.Task;
        }

        public static Task<T> CancelFault<T>()
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetException(cancelException);
            return tcs.Task;
        }
    }
}
