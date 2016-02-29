using System;
using C = System.Collections;
using G = System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ZBrad.AsyncLib
{
    internal class NodesEnum<N> : G.IEnumerator<N> where N : INode
    {
        INodeCollection<N> nodes;
        int version;
        INode cur;
        bool isInit = false;

        public N Current { get { return (N)cur; } }

        public NodesEnum(INodeCollection<N> nodes)
        {
            this.nodes = nodes;
            this.version = nodes.Version;
            this.Reset();
        }

        public bool MoveNext()
        {
            if (version != nodes.Version)
                throw new InvalidOperationException("collection modified");

            if (isInit)
            {
                if (cur == nodes.Root)
                    return false;
                return (cur = cur.Next) != nodes.Root;
            }

            this.isInit = true;
            this.cur = nodes.Root;
            return true;
        }

        public void Reset()
        {
            if (version != nodes.Version)
                throw new InvalidOperationException("collection modified");

            this.isInit = false;
        }

        #region explicit interface implementations

        object C.IEnumerator.Current { get { return this.Current; } }

        void IDisposable.Dispose()
        {
        }

        #endregion
    }

}
