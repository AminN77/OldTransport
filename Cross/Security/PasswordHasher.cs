using Cross.Abstractions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Cross.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password, byte[] salt, int iterations, int outputCount)
        {
            byte[] hashByteArray;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations))
            {
                hashByteArray = pbkdf2.GetBytes(outputCount / 2);
            }
            var stringBuilder = new StringBuilder();
            foreach (var item in hashByteArray)
            {
                stringBuilder.Append(item.ToString("X2"));
            }
            return stringBuilder.ToString();
        }

        public async Task<string> HashPasswordAsync(string password, byte[] salt, int iterations, int outputCount)
        {
            byte[] hashByteArray;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations))
            {
                hashByteArray = await Task.Run(() => pbkdf2.GetBytes(outputCount / 2));
            }
            var stringBuilder = new StringBuilder();
            await Task.Run(() =>
            {
                foreach (var item in hashByteArray)
                {
                    stringBuilder.Append(item.ToString("X2"));
                }
            });
            return stringBuilder.ToString();
        }
    }
}
