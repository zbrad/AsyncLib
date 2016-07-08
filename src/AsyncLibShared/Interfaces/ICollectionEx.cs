using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZBrad.AsyncLib.Collections
{
    /// <summary>
    /// a more data structure independent version if ICollection.  Doesn't presume Add/Remove behaviors.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICollectionEx<T> : IEnumerable<T>
    {
        int Count { get; }
        void CopyTo(T[] array, int index);
    }
}
