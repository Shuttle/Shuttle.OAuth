using System.Threading.Tasks;

namespace Shuttle.OAuth;

public interface ICodeChallenge
{
    string Method { get; }
    ValueTask<string> GenerateCodeChallengeAsync(string codeVerifier);
    ValueTask<string> GenerateCodeVerifierAsync();
}