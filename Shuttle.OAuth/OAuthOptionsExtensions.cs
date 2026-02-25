using Shuttle.Core.Contract;

namespace Shuttle.OAuth;

public static class OAuthOptionsExtensions
{
    extension(OAuthOptions oauthOptions)
    {
        public OAuthProviderOptions? FindProviderOptions(string providerName)
        {
            Guard.AgainstEmpty(providerName);

            return oauthOptions.Providers.TryGetValue(providerName, out var providerOptions)
                ? providerOptions
                : null;
        }

        public OAuthProviderOptions GetProviderOptions(string providerName)
        {
            return oauthOptions.FindProviderOptions(providerName) ?? throw new InvalidOperationException(string.Format(Resources.OAuthProviderOptionsNotFoundException, providerName));
        }
    }
}