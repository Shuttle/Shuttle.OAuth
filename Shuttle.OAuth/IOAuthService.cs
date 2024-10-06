using System;
using System.Threading.Tasks;

namespace Shuttle.OAuth
{
    public interface IOAuthService
    {
        Task<OAuthGrant> RegisterAsync(string providerName);
        Task<dynamic> GetDataAsync(OAuthGrant grant, string code);
    }
}