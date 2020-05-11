using System;
using System.ComponentModel.DataAnnotations.Schema;
using Data.Abstractions.Models;

namespace Data.Model
{
    public class UserToken : IUserToken
    {
        public int Id { get; set; }

        public string AccessTokenHash { get; set; }

        public DateTimeOffset AccessTokenExpiresDateTime { get; set; }

        public string RefreshTokenIdHash { get; set; }

        public string RefreshTokenIdHashSource { get; set; }

        public DateTimeOffset RefreshTokenExpiresDateTime { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; } // one-to-one association

        public virtual User User { get; set; }
    }
}
