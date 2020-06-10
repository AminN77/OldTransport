namespace Data.Abstractions.Models
{
    interface ITransporter : IEntity<int>
    {
        string Bio { get; set; }
    }
}