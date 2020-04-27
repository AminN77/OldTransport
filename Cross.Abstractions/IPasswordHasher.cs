using System.Threading.Tasks;

namespace Cross.Abstractions
{
    public interface IPasswordHasher
    {
        string HashPassword(string password, byte[] salt, int iterations, int outputCount);
        Task<string> HashPasswordAsync(string password, byte[] salt, int iterations, int outputCount);
    }
}
