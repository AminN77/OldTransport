using System;
using System.Threading.Tasks;
using Cross.Abstractions;

namespace BusinessLogic.Abstractions
{
    public interface ISecurityProvider : IDisposable
    {
        IPasswordHasher PasswordHasher { get; set; }
        Task<string> Md5HashAsync(string inputString);
        Task<string> Sha256HashAsync(string inputString);
        Task<byte[]> GenerateRandomSaltAsync(int saltCount);
    }
}
