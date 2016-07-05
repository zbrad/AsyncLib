namespace ZBrad.AsyncLib.Collections
{
    internal class Try<T> : ITry<T>
    {
        public static Try<T> False = new Try<T> { Result = false };

        public bool Result { get; private set; }

        public T Value { get; private set; }

        Try() {}

        public Try(T value)
        {
            this.Result = true;
            this.Value = value;
        }
    }
}
