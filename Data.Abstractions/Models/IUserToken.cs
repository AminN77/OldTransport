using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Abstractions.Models
{
    public interface IUserToken : IEntity<int>
    {
        string AccessTokenHash { get; set; }

        DateTimeOffset AccessTokenExpiresDateTime { get; set; }

        string RefreshTokenIdHash { get; set; }

        DateTimeOffset RefreshTokenExpiresDateTime { get; set; }
    }
}
