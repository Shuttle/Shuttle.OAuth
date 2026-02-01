using Shuttle.Core.Contract;

namespace Shuttle.OAuth;

public static class OAuthOptionsExtensions
{
    extension(OAuthOptions oauthOptions)
    {
        public OAuthProviderOptions? FindProviderOptions(string providerName)
        {
            Guard.AgainstEmpty(providerName);

            return Guard.AgainstNull(oauthOptions).Providers.FirstOrDefault(provider => provider.Name.Equals(providerName, StringComparison.InvariantCultureIgnoreCase));
        }

        public OAuthProviderOptions GetProviderOptions(string providerName)
        {
            return oauthOptions.FindProviderOptions(providerName) ?? throw new InvalidOperationException(string.Format(Resources.OAuthProviderOptionsNotFoundException, providerName));
        }
    }
}