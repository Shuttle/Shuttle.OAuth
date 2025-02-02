using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shuttle.OAuth;

public interface IOAuthService
{
    Task<dynamic> GetDataAsync(OAuthGrant grant, string code);
    Task<OAuthGrant> RegisterAsync(string providerName, IDictionary<string, string>? data = null);
}