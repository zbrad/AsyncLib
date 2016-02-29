using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZBrad.AsyncLib.Public.Classes
{
    public class WaitList<N> : IListAsync<N>, IWaitable where N : class, INode
    {
        NodeList<N> list = new NodeList<N>();
        bool isEnded = false;
        static Task<N> Cancelled { get { return (Task<N>)TaskEx.Cancelled; } }


        // we have to use list to allow cancel cleanup
        NodeList<Waiter<N>> waiters = new NodeList<Waiter<N>>();

        public int WaitCount { get { return waiters.Count; } }

        AwaitLock qlock = new AwaitLock();
        public AwaitLock Locker { get { return this.qlock; } }

    }
}
