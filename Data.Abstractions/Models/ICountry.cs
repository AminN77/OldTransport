namespace Data.Abstractions.Models
{
    public interface ICountry : IEntity<int>
    {
        string Name{ get; set; }
    }
}
