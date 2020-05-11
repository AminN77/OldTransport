using System;

namespace MvcWebApi.Providers
{
    public interface ISecurityService
    {
        Guid CreateCryptographicallySecureGuid();
    }
}