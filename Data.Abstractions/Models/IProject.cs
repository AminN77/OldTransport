using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Abstractions.Models
{
    public interface IProject : IEntity<int>
    {
        string Description { get; set; }

        string BeginningCountry { get; set; }

        string BeginningCity { get; set; }

        string DestinationCountry { get; set; }

        string DestinationCity { get; set; }

        string Title { get; set; }

        string Cargo { get; set; }

        DateTime CreateDateTime { get; set; }

        int Budget { get; set; }

        double Weight { get; set; }

        double Quantity { get; set; }

        double Dimention { get; set; }
    }
}
