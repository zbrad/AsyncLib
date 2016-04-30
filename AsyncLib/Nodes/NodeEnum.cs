using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using G = System.Collections.Generic;
using ZBrad.AsyncLib.Links;

namespace ZBrad.AsyncLib.Nodes
{
    internal class NodeEnum<T> : G.IEnumerator<T> where T : ILink
    {
        INodeCollection<T> values;
        long version;
        ILink root;
        ILink cur;

        public T Current { get { return (T) cur; } }

        public NodeEnum(INodeCollection<T> values)
        {
            this.values = values;
            this.root = values.Root;
            this.version = values.Version;
            this.Reset();
        }

        public bool MoveNext()
        {
            if (version != values.Version)
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
            this.cur = null;
        }

        #region explicit interface implementations

        object System.Collections.IEnumerator.Current { get { throw new NotImplementedException("use typed Current property"); } }

        void IDisposable.Dispose()
        {
            this.values = null;
            this.cur = null;
        }

        #endregion
    }

}
