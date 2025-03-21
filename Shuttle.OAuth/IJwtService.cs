using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;

namespace Shuttle.OAuth;

public interface IJwtService
{
    ValueTask<string> GetIdentityNameAsync(string token);
    Task<TokenValidationResult> ValidateTokenAsync(string token);
}