using System;

namespace Data.Abstractions.Models
{
    public interface IUser: IEntity<int>
    {
        string Name { get; set; }
        string EmailAddress { get; set; }
        string Password { get; set; }
        string Picture { get; set; }
        bool IsEnabled { get; set; }
        bool IsDeleted { get; set; }
        byte[] Salt { get; set; }
        int IterationCount { get; set; }
        int ActivationCode { get; set; }
        string SerialNumber { get; set; }
        DateTimeOffset? LastLoggedIn { get; set; }
        DateTime CreateDateTime { get; set; }
    }
}