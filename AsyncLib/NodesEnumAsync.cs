using System;
using C = System.Collections;
using G = System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZBrad.AsyncLib
{
    internal class NodesEnumAsync<N> : INodeEnumeratorAsync<N> where N : INode
    {
        static readonly Task<bool> False = Task.FromResult<bool>(false);
        static readonly Task<bool> True = Task.FromResult<bool>(true);

        INodeCollectionAsync<N> nodes;
        int version;
        INode cur;
        bool isInit = false;

        public N Current { get { return (N) cur; } }

        public NodesEnumAsync(INodeCollectionAsync<N> nodes)
        {
            this.nodes = nodes;
            this.version = nodes.Version;
            this.Reset();
        }

        public Task<bool> MoveNext()
        {
            if (version != nodes.Version)
                throw new InvalidOperationException("collection modified");

            if (isInit)
            {
                if (cur == null)
                    return False;
                return Task.FromResult<bool>((cur = cur.Next) != null);
            }

            this.isInit = true;
            this.cur = nodes.Root;
            return True;
        }

        public Task Reset()
        {
            if (version != nodes.Version)
                throw new InvalidOperationException("collection modified");

            this.isInit = false;
            return True;
        }
    }

}
