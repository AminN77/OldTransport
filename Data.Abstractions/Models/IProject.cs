﻿using System;
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

        DateTime CreateDateTime { get; set; }

        double Budget { get; set; }

        double Weight { get; set; }


    }
}
