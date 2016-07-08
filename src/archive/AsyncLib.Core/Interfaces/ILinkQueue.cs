using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZBrad.AsyncLib.Collections
{
    public interface ILinkQueue<T> : IQueue<T> where T : ILink, IEquatable<T>
    {
        void Remove(ILink link);
        ITry<T> TryHead(T value);
        ITry<T> TryTail(T value);
    }

    public interface IOrderedLinkQueue<T> : ILinkQueue<T> where T : ILink, IEquatable<T>, IComparable<T>
    {

    }
}
