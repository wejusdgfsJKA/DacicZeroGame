namespace Pooling
{
    public interface IPoolable<Id>
    {
        Id ID { get; }
    }
}