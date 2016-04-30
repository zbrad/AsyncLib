//using S = System;
//using System.Threading;
//using System.Threading.Tasks;

//namespace ZBrad.AsyncLib.Nodes
//{
//    public interface ILinkedList<N> : IValueCollection<N> where N : S.IEquatable<N>
//    {
//        IValue<N> Head { get; }
//        IValue<N> Tail { get; }

//        bool InsertAtHead(N value);
//        bool InsertAtTail(N value);

//        N RemoveFromHead();
//        N RemoveFromTail();
//    }

//    public interface ILinkedListAsync<N> : IValueAsyncCollection<N> where N : S.IEquatable<N>
//    {
//        IValue<N> Head { get; }
//        IValue<N> Tail { get; }

//        Task<bool> InsertAtHeadAsync(N value);
//        Task<bool> InsertAtHeadAsync(N value, CancellationToken token);

//        Task<bool> InsertAtTailAsync(N value);
//        Task<bool> InsertAtTailAsync(N value, CancellationToken token);

//        Task<N> RemoveFromHeadAsync();
//        Task<N> RemoveFromHeadAsync(CancellationToken token);

//        Task<N> RemoveFromTailAsync();
//        Task<N> RemoveFromTailAsync(CancellationToken token);
//    }

//    // adds IComparable constraint
//    public interface IListOrdered<N> : ILinkedList<N>, IValueCollection<N> where N : S.IEquatable<N>, S.IComparable<N>
//    {

//    }

//    // adds IComparable constraint
//    public interface IListOrderedAsync<N> : ILinkedListAsync<N>, IValueAsyncCollection<N> where N : S.IEquatable<N>, S.IComparable<N>
//    {

//    }

//}
