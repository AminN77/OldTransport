using System;

namespace Data.Abstractions.Models
{
    public interface IContactUs : IEntity<int>
    {
        string EmailAddress { get; set; }

        string Name { get; set; }

        string Text { get; set; }

        DateTime CreateDateTime { get; set; }
    }
}
