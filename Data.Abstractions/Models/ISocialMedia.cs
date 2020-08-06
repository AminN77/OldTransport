namespace Data.Abstractions.Models
{
    public interface ISocialMedia : IEntity<int>
    {
        string Name { get; set; }

        string Link { get; set; }
    }
}
