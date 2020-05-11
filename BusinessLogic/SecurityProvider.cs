using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Abstractions;
using Cross.Abstractions;
using Data.Abstractions;
using Data.Abstractions.Models;
using Data.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BusinessLogic
{
    public class SecurityProvider : ISecurityProvider
    {
        private bool _disposed;
        private MD5 _md5;
        private SHA256 _sha256;
        private RNGCryptoServiceProvider _rngCryptoServiceProvider;
        private IRepository<User> UserRepository { get; set; }
        public IPasswordHasher PasswordHasher { get; set; }
        public ILogger<SecurityProvider> _logger { get; set; }


        public SecurityProvider(IPasswordHasher passwordHasher,
            IRepository<User> userRepository,
            ILogger<SecurityProvider> logger)
        {
            PasswordHasher = passwordHasher;
            UserRepository = userRepository;
            _md5 = MD5.Create();
            _sha256 = SHA256.Create();
            _rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            _logger = logger;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                UserRepository?.Dispose();
                _md5?.Dispose();
                _sha256?.Dispose();
                _rngCryptoServiceProvider?.Dispose();
            }

            UserRepository = null;
            PasswordHasher = null;
            _md5 = null;
            _sha256 = null;
            _rngCryptoServiceProvider = null;
            _disposed = true;
        }

        private static async Task<string> GetHexadecimalString(byte[] inputbytes)
        {
            var stringBuilder = new StringBuilder();
            await Task.Run(() =>
            {
                foreach (var item in inputbytes)
                {
                    stringBuilder.Append(item.ToString("X2"));
                }
            });
            return stringBuilder.ToString();
        }

        public async Task<string> Md5HashAsync(string inputString)
        {
            var hashByteArray = await Task.Run(() => _md5.ComputeHash(Encoding.UTF8.GetBytes(inputString)));
            return await GetHexadecimalString(hashByteArray);
        }

        public async Task<string> Sha256HashAsync(string inputString)
        {
            var hashByteArray = await Task.Run(() => _sha256.ComputeHash(Encoding.UTF8.GetBytes(inputString)));
            return await GetHexadecimalString(hashByteArray);
        }

        public async Task<byte[]> GenerateRandomSaltAsync(int saltCount)
        {
            var salt = new byte[saltCount];
            await Task.Run(() => _rngCryptoServiceProvider.GetBytes(salt));
            return salt;
        }

        public byte[] GenerateRandomSalt(int saltCount)
        {
            var salt = new byte[saltCount];
            _rngCryptoServiceProvider.GetBytes(salt);
            return salt;
        }

        public IUser IsUserAuthenticate(string emailAddress, string password)
        {
            var user = UserRepository.DeferredSelectAll()
                .SingleOrDefault(usr => usr.EmailAddress == emailAddress && usr.IsEnabled);
            if (user == null) return null;
            var passwordHash = PasswordHasher.HashPassword(password, user.Salt, user.IterationCount, 128);
            return user.Password == passwordHash ? user : null;
        }

        public async Task<IUser> IsUserAuthenticateAsync(string emailAddress, string password)
        {
            var user = await UserRepository.DeferredSelectAll().SingleOrDefaultAsync(usr => usr.EmailAddress == emailAddress && usr.IsEnabled);
            if (user == null) return null;
            var passwordHash = await PasswordHasher.HashPasswordAsync(password, user.Salt, user.IterationCount, 128);
            return user.Password == passwordHash ? user : null;
        }

        ~SecurityProvider()
        {
            Dispose(false);
        }
    }
}