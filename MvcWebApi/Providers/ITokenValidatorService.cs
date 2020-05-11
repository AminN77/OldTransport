using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace MvcWebApi.Providers
{
    public interface ITokenValidatorService
    {
        Task ValidateAsync(TokenValidatedContext context);
    }
}
