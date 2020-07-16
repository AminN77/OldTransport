using Cross.Abstractions.EntityEnums;

namespace Data.Abstractions.Models
{
    public interface IAccept : IEntity<int>
    {
        AcceptStatus Status { get; set; }
    }
}
