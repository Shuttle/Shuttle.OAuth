using System;
using System.Collections.Generic;
using Shuttle.Core.Contract;

namespace Shuttle.OAuth;

public class OAuthGrant
{
    public OAuthGrant(Guid id, string providerName, IDictionary<string, string>? data = null)
    {
        Id = id;
        Data = data ?? new Dictionary<string, string>();
        ProviderName = Guard.AgainstNullOrEmptyString(providerName);
    }

    public string CodeChallenge { get; private set; } = default!;
    public string CodeVerifier { get; private set; } = default!;

    public Guid Id { get; }
    public IDictionary<string, string> Data { get; }
    public string ProviderName { get; }

    public OAuthGrant WithCodeChallenge(string codeChallenge, string codeVerifier)
    {
        CodeChallenge = Guard.AgainstNullOrEmptyString(codeChallenge);
        CodeVerifier = Guard.AgainstNullOrEmptyString(codeVerifier);

        return this;
    }

    public bool HasData(string name)
    {
        return Data.ContainsKey(Guard.AgainstNullOrEmptyString(name));
    }

    public string GetData(string name)
    {
        if (!HasData(name))
        {
            throw new InvalidOperationException(string.Format(Resources.OAuthGrantDataNameNotFoundException, name));
        }

        return Data[Guard.AgainstNullOrEmptyString(name)];
    }
}