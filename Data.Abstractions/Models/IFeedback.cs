using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Abstractions.Models
{
    public interface IFeedback : IEntity<int>
    {
        string EmailAddress { get; set; }

        string Name { get; set; }

        string Text { get; set; }

        DateTime CreateDateTime { get; set; }
    }
}
