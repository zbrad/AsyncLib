using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZBrad.AsyncLib
{
    public static class TaskEx
    {
        public static Task<bool> FaultedBool;
        public static Task<object> FaultedNull;
        public static Task<bool> True = Task<bool>.FromResult<bool>(true);
        public static Task<bool> False = Task<bool>.FromResult<bool>(false);
        public static Task Faulted = FaultedNull;

        public static Task<bool> CanceledBool;
        public static Task Canceled;

        static TaskEx()
        {
            // cancelled tasks
            var tCancel = new TaskCompletionSource<bool>();
            tCancel.SetCanceled();
            CanceledBool = tCancel.Task;
            Canceled = tCancel.Task;

            // faulted with "task canceled" tasks
            var e = new TaskCanceledException();
            var tBool = new TaskCompletionSource<bool>();
            tBool.SetException(e);
            FaultedBool = tBool.Task;

            var tNull = new TaskCompletionSource<object>();
            tNull.SetException(e);
            FaultedNull = tNull.Task;

            // generic task cancelled
            Faulted = FaultedNull;
        }
    }
}
