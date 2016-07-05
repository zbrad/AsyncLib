using System;
using System.Collections;
using System.Collections.Generic;

namespace ZBrad.AsyncLib.Collections
{
    internal class LinkQueue<T> : ILinkQueue<T> where T : ILink, IEquatable<T>
    {
        LinkList<T> list;

        public LinkQueue()
        {
            list = new LinkList<T>();
        }

        public int Count { get { return list.Count; } }

        public ILink Root { get { return list.Head; } }

        public void Clear()
        {
            list.Clear();
        }

        public void CopyTo(T[] array, int index)
        {
            list.CopyTo(array, index);
        }

        public T Dequeue()
        {
            return list.RemoveFromHead();
        }

        public void Enqueue(T item)
        {
            list.InsertAtTail(item);
        }

        public T Peek()
        {
            return list.Head;
        }

        public bool Contains(T value)
        {
            var trial = TryTail(value);
            return trial.Result;
        }

        public void Remove(ILink link)
        {
            list.Remove(link);
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

        public ITry<T> TryHead(T value)
        {
            return list.TryNext(list.Head, value);
        }

        public ITry<T> TryTail(T value)
        {
            return list.TryPrev(list.Tail, value);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
