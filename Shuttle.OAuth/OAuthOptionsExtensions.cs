using System;
using System.Linq;
using Shuttle.Core.Contract;

namespace Shuttle.OAuth;

public static class OAuthOptionsExtensions
{
    public static OAuthProviderOptions? FindProviderOptions(this OAuthOptions oauthOptions, string providerName)
    {
        Guard.AgainstNullOrEmptyString(providerName);

        return Guard.AgainstNull(oauthOptions).Providers.FirstOrDefault(provider => provider.Name.Equals(providerName, StringComparison.InvariantCultureIgnoreCase));
    }

    public static OAuthProviderOptions GetProviderOptions(this OAuthOptions oauthOptions, string providerName)
    {
        return oauthOptions.FindProviderOptions(providerName) ?? throw new InvalidOperationException(string.Format(Resources.OAuthProviderOptionsNotFoundException, providerName));
    }
}