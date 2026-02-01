using Shuttle.Core.Contract;

namespace Shuttle.OAuth;

public class OAuthGrant(Guid id, string providerName, IDictionary<string, string>? data = null)
{
    public string CodeChallenge { get; private set; } = string.Empty;
    public string CodeVerifier { get; private set; } = string.Empty;
    public IDictionary<string, string> Data { get; } = data ?? new Dictionary<string, string>();

    public Guid Id { get; } = Guard.AgainstEmpty(id);
    public string ProviderName { get; } = Guard.AgainstEmpty(providerName);

    public string GetData(string name)
    {
        return !HasData(name) ? throw new InvalidOperationException(string.Format(Resources.OAuthGrantDataNameNotFoundException, name)) : Data[Guard.AgainstEmpty(name)];
    }

    public bool HasData(string name)
    {
        return Data.ContainsKey(Guard.AgainstEmpty(name));
    }

    public OAuthGrant WithCodeChallenge(string codeChallenge, string codeVerifier)
    {
        CodeChallenge = Guard.AgainstEmpty(codeChallenge);
        CodeVerifier = Guard.AgainstEmpty(codeVerifier);

        return this;
    }
}