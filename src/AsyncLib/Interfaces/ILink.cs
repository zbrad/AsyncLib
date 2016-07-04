namespace ZBrad.AsyncLib
{
    public interface ILink
    {
        ILink Prev { get; set; }
        ILink Next { get; set; }
    }
}
