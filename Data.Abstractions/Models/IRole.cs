namespace Data.Abstractions.Models
{
    public interface IRole : IEntity<int>
    {
        string Name { get; set; }
    }
}
