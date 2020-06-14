using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Abstractions.Models
{
    public interface IOffer
    {
        string Description { get; set; }

        double Price { get; set; }

        int EstimatedTime { get; set; }

        DateTime CreateDate { get; set; }
    }
}
