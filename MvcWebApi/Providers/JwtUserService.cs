using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Data.Model;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ViewModels;

namespace MvcWebApi.Providers
{
    public interface IUserService
    {
        Task<UserSignInViewModel> Authenticate(UserSignInViewModel user);
        //IEnumerable<User> GetAll();
    }

    public class UserService : IUserService
    {
        private readonly AppSettings _appSettings;

        public UserService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        // Todo: this commented line go to web api controller.
//        private readonly IBusinessLogicUserManager _userRepository;
//
//        public UserService(IBusinessLogicUserManager userRepository)
//        {
//            _userRepository = userRepository;
//        }

        public async Task<UserSignInViewModel> Authenticate(UserSignInViewModel user)
        {
            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);
            return user;
        }
//
//        public IEnumerable<User> GetAll()
//        {
//            return _users;
//        }
    }
}