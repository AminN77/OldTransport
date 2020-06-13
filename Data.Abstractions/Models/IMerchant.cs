namespace Data.Abstractions.Models
{
    public interface IMerchant : IEntity<int>
    {
        string Bio { get; set; }
    }
}
