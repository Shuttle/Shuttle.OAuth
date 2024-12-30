using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Shuttle.Core.Contract;

namespace Shuttle.OAuth;

public class S256CodeChallenge : ICodeChallenge
{
    public string Method => "S256";

    public async ValueTask<string> GenerateCodeVerifierAsync()
    {
        var bytes = new byte[32];
        RandomNumberGenerator.Fill(bytes);
        return await ValueTask.FromResult(Base64UrlEncode(bytes));
    }

    public async ValueTask<string> GenerateCodeChallengeAsync(string codeVerifier)
    {
        Guard.AgainstNullOrEmptyString(codeVerifier);

        using (var sha256 = SHA256.Create())
        {
            var bytes = Encoding.ASCII.GetBytes(codeVerifier);
            var hash = sha256.ComputeHash(bytes);
            return await ValueTask.FromResult(Base64UrlEncode(hash));
        }
    }

    private static string Base64UrlEncode(byte[] input)
    {
        return Convert.ToBase64String(input)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }
}