using System;
using System.Threading.Tasks;

namespace Shuttle.OAuth
{
    public interface IOAuthService
    {
        Task<OAuthGrant> RegisterAsync(string providerName);
        Task<dynamic?> GetDataDynamicAsync(Guid requestId, string code);
    }
}