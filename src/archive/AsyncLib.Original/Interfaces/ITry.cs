namespace ZBrad.AsyncLib.Collections
{
    public interface ITry<T>
    {
        bool Result { get; }
        T Value { get; }
    }
}
