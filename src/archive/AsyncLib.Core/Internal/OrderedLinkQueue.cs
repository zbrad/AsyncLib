using C = System.Collections;
using G = System.Collections.Generic;
using System;

namespace ZBrad.AsyncLib.Collections
{
    internal class OrderedLinkQueue<T> : IOrderedLinkQueue<T> where T : ILink, IEquatable<T>, IComparable<T>
    {
        OrderedLinkList<T> list;

        public OrderedLinkQueue()
        {
            list = new OrderedLinkList<T>();
        }

        public int Count => list.Count;

        public void Clear()
        {
            list.Clear();
        }

        public bool Contains(T item)
        {
            var trial = TryTail(item);
            return trial.Result;
        }

        public virtual void Enqueue(T value)
        {
            list.InsertAtTail(value);
        }

        public virtual T Dequeue()
        {
            return list.RemoveFromHead();
        }

        public virtual T Peek()
        {
            if (list.Count == 0)
                return default(T);

            return list.Head;
        }

        public ITry<T> TryHead(T value)
        {
            return list.TryNext(list.Head, value);
        }

        public ITry<T> TryTail(T value)
        {
            return list.TryPrev(list.Tail, value);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public T[] ToArray()
        {
            var array = new T[this.Count];
            CopyTo(array, 0);
            return array;
        }

        public void TrimExcess()
        {
            // no op
        }

        public void Remove(ILink link)
        {
            list.Remove(link);
        }

        public G.IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        C.IEnumerator C.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
