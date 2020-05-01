using System;

namespace Data.Abstractions.Models
{
    public interface IUser: IEntity<int>
    {
        string FirstName { get; set; }
        string LastName { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        string EmailAddress { get; set; }
        string Picture { get; set; }
        bool IsEnabled { get; set; }
        bool IsDeleted { get; set; }
        string Token { get; set; }
        DateTime CreateDateTime { get; set; }
    }
}