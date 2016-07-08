using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZBrad.AsyncLib.Collections
{
    /// <summary>
    /// intended as a wrapper for the generic Queue<T> class which does not provide an interface abstraction
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IQueue<T> : ICollectionEx<T>
    {
        void Clear();
        bool Contains(T item);
        T Dequeue();
        void Enqueue(T item);
        T Peek();
        T[] ToArray();
        void TrimExcess();
    }
}
