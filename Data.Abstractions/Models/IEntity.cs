namespace Data.Abstractions.Models
{
    public interface IEntity<out TKey>
    {
        TKey Id { get; }
    }
}
