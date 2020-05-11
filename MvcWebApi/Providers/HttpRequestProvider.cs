using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace MvcWebApi.Provider
{
    public static class HttpRequestProvider
    {
        public static int GetCurrentUserId(this HttpContext context)
        {
            try
            {
                return Convert.ToInt32(context.User.FindFirstValue(ClaimTypes.NameIdentifier));
            }
#pragma warning disable CS0168 // Variable is declared but never used
            catch (Exception exception)
#pragma warning restore CS0168 // Variable is declared but never used
            {
                // Log exception
                return 0;
            }
        }

        public static string GetCurrentUserName(this HttpContext context)
        {
            try
            {
                return context.User.FindFirstValue(ClaimTypes.Name);
            }
#pragma warning disable CS0168 // Variable is declared but never used
            catch (Exception exception)
#pragma warning restore CS0168 // Variable is declared but never used
            {
                // Log exception
                return null;
            }
        }
    }
}