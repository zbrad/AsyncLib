using System;
using C = System.Collections;
using G = System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ZBrad.AsyncLib.Nodes
{
    internal class NodeEnumAsync<T> : IValueAsyncEnumerator<T> where T : IEquatable<T>
    {
        IValueAsyncCollection<T> values;
        long version;
        IValue<T> root;
        IValue<T> cur;

        public NodeEnumAsync(IValueAsyncCollection<T> values)
        {
            this.values = values;
            this.root = values.Root;
            this.version = values.Version;
            this.cur = null;
            this.Reset();
        }

        public T Current { get { return cur.Value; } }

        public Task<bool> MoveNextAsync()
        {
            return this.MoveNextAsync(CancellationToken.None);
        }

        public async Task<bool> MoveNextAsync(CancellationToken token)
        {
            using (await values.Locker.WaitAsync(token))
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

                cur = (IValue<T>)cur.Next;
                return true;
            }
        }

        public void Reset()
        {
            this.cur = null;
        }
    }
}
