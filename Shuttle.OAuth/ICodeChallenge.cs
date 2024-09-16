using System.Threading.Tasks;

namespace Shuttle.OAuth
{
    public interface ICodeChallenge
    {
        string Method { get; }
        ValueTask<string> GenerateCodeVerifierAsync();
        ValueTask<string> GenerateCodeChallengeAsync(string codeVerifier);
    }
}