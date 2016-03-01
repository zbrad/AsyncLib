using System;
using C = System.Collections;
using G = System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ZBrad.AsyncLib
{
    internal class NodesEnumAsync<N> : IAsyncEnumerator<N> where N : INode, IEquatable<N>
    {
        INodeCollectionAsync<N> nodes;
        NodesEnum<N> nodesEnum;

        public NodesEnumAsync(INodeCollectionAsync<N> nodes)
        {
            this.nodes = nodes;
            this.nodesEnum = new NodesEnum<N>(nodes);
        }

        public N Current { get { return nodesEnum.Current; } }

        public Task<bool> MoveNextAsync()
        {
            return this.MoveNextAsync(CancellationToken.None);
        }

        public async Task<bool> MoveNextAsync(CancellationToken token)
        {
            using (await nodes.Locker.WaitAsync(token))
            {
                return nodesEnum.MoveNext();
            }
        }

        public void Reset()
        {
            nodesEnum.Reset();
        }

        #region explicit interface implementations

        bool C.IEnumerator.MoveNext() { throw new NotImplementedException("use MoveNextAsync for async structure"); }

        object C.IEnumerator.Current { get { throw new NotImplementedException("use typed Current property");  } }

        void IDisposable.Dispose()
        {
            nodes = null;
            if (nodesEnum != null)
                ((IDisposable)nodesEnum).Dispose();
            nodesEnum = null;
        }

        #endregion
    }

}
