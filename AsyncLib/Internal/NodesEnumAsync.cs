using System;
using C = System.Collections;
using G = System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZBrad.AsyncLib
{
    internal class NodesEnumAsync<N> : INodeEnumeratorAsync<N> where N : class, INode
    {
        static readonly Task<bool> False = Task.FromResult<bool>(false);
        static readonly Task<bool> True = Task.FromResult<bool>(true);

        INodeCollectionAsync<N> nodes;
        int version;
        INode cur;

        public N Current { get { return cur as N; } }

        public NodesEnumAsync(INodeCollectionAsync<N> nodes)
        {
            this.nodes = nodes;
            this.version = nodes.Version;
            this.ResetAsync().Wait();
        }

        public async Task<bool> MoveNextAsync()
        {
            using (await nodes.Locker.WaitAsync())
            {
                if (version != nodes.Version)
                    throw new InvalidOperationException("collection modified");
                cur = cur.Next;
            }

            return cur != null;
        }

        public async Task ResetAsync()
        {
            this.cur = await nodes.GetRootAsync(this.version);
        }
    }

}
