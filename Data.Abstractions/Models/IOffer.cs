using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Abstractions.Models
{
    public interface IOffer: IEntity<int>
    {
        string Description { get; set; }

        int Price { get; set; }

        int EstimatedTime { get; set; }

        DateTime CreateDate { get; set; }
    }
}
