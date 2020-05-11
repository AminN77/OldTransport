using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace MvcWebApi.Providers
{
    public class AntiForgeryCookieService : IAntiForgeryCookieService
    {
        // Variables
        private const string XsrfTokenKey = "XSRF-TOKEN";
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IAntiforgery _antiForgery;
        private readonly IOptions<AntiforgeryOptions> _antiForgeryOptions;

        // Ctor
        public AntiForgeryCookieService(
            IHttpContextAccessor contextAccessor,
            IAntiforgery antiForgery,
            IOptions<AntiforgeryOptions> antiForgeryOptions)
        {
            _contextAccessor = contextAccessor;
            _antiForgery = antiForgery;
            _antiForgeryOptions = antiForgeryOptions;
        }

        // RegenerateAntiForgeryCookies
        public void RegenerateAntiForgeryCookies(IEnumerable<Claim> claims)
        {
            var httpContext = _contextAccessor.HttpContext;
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme));
            var tokens = _antiForgery.GetAndStoreTokens(httpContext);
            httpContext.Response.Cookies.Append(
                key: XsrfTokenKey,
                value: tokens.RequestToken,
                options: new CookieOptions
                {
                    HttpOnly = false // Now JavaScript is able to read the cookie
                });
        }

        // DeleteAntiForgeryCookies
        public void DeleteAntiForgeryCookies()
        {
            var cookies = _contextAccessor.HttpContext.Response.Cookies;
            cookies.Delete(_antiForgeryOptions.Value.Cookie.Name);
            cookies.Delete(XsrfTokenKey);
        }
    }
}