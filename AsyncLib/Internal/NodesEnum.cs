using System;
using C = System.Collections;
using G = System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ZBrad.AsyncLib
{
    internal class NodesEnum<N> : G.IEnumerator<N> where N : INode, IEquatable<N>
    {
        INodeCollection<N> nodes;
        long version;
        INode root;
        INode cur;
        bool isInit = false;

        public N Current { get { return (N)cur; } }
        public NodesEnum(INodeCollection<N> nodes)
        {
            this.nodes = nodes;
            this.root = nodes.Root;
            this.version = nodes.Version;
            this.cur = null;
            this.Reset();
        }

        public bool MoveNext()
        {
            if (version != nodes.Version)
                throw new InvalidOperationException("collection modified");

            if (root == null)
                return false;

            if (cur == null)
            {
                cur = root;
                return true;
            }

            if (cur.Next == root)
                return false;

            cur = cur.Next;
            return true;
        }

        public void Reset()
        {
            if (version != nodes.Version)
                throw new InvalidOperationException("collection modified");

            this.cur = null;
        }

        #region explicit interface implementations

        object C.IEnumerator.Current { get { throw new NotImplementedException("use typed Current property"); } }

        void IDisposable.Dispose()
        {
            this.nodes = null;
            this.cur = null;
        }

        #endregion
    }

}
