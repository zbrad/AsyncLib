using System.Threading.Tasks;

namespace ZBrad.AsyncLib
{
    public static class Extensions
    {
        public static void WaitEx(this Task t)
        {
            t.GetAwaiter().GetResult();
        }

        public static T ResultEx<T>(this Task<T> t)
        {
            return t.GetAwaiter().GetResult();
        }
    }
}
