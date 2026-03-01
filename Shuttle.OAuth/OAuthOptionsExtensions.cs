using Shuttle.Core.Contract;

namespace Shuttle.OAuth;

public static class OAuthOptionsExtensions
{
    extension(OAuthOptions oauthOptions)
    {
        public OAuthProviderOptions? FindProviderOptions(string provider)
        {
            Guard.AgainstEmpty(provider);

            return oauthOptions.Providers.TryGetValue(provider, out var providerOptions)
                ? providerOptions
                : null;
        }

        public OAuthProviderOptions GetProviderOptions(string provider)
        {
            return oauthOptions.FindProviderOptions(provider) ?? throw new InvalidOperationException(string.Format(Resources.OAuthProviderOptionsNotFoundException, provider));
        }
    }
}