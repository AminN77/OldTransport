using System;
using System.Threading.Tasks;

namespace Cross.Abstractions
{
    public interface IEmailSender
    {
        Task<bool> Send(string To, string Subject, string Body);
    }
}
