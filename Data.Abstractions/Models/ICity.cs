namespace Data.Abstractions.Models
{
    public interface ICity : IEntity<int>
    {
        string Name { get; set; }
    }
}
