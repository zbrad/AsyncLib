using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZBrad.AsyncLib
{
    public static class TaskEx
    {
        public static Task<bool> CancelledBool;
        public static Task<object> CancelledNull;
        public static Task<bool> True = Task.FromResult<bool>(true);
        public static Task<bool> False = Task.FromResult<bool>(false);
        public static Task Cancelled { get { return CancelledBool; } }

        static TaskEx()
        {
            var tcs = new TaskCompletionSource<bool>();
            tcs.SetCanceled();
            CancelledBool = tcs.Task;

            var tnull = new TaskCompletionSource<object>();
            tnull.SetCanceled();
            CancelledNull = tnull.Task;
        }
    }
}
