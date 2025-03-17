using System.Threading.Tasks;

namespace Shuttle.OAuth;

public interface IJwtService
{
    ValueTask<string> GetIdentityNameAsync(string token);
    ValueTask<bool> IsValidAsync(string token);
}