using Shuttle.Core.Contract;

namespace Shuttle.OAuth;

public static class OAuthOptionsExtensions
{
    extension(OAuthOptions oauthOptions)
    {
        public OAuthProviderOptions? FindProviderOptions(string groupName, string providerName)
        {
            Guard.AgainstEmpty(groupName);
            Guard.AgainstEmpty(providerName);

            if (!oauthOptions.TryGetValue(groupName, out var providerGroupOptions))
            {
                return null;
            }

            return providerGroupOptions.Providers.TryGetValue(providerName, out var providerOptions)
                ? providerOptions
                : null;
        }

        public OAuthProviderOptions GetProviderOptions(string groupName, string providerName)
        {
            return oauthOptions.FindProviderOptions(groupName, providerName) ?? throw new InvalidOperationException(string.Format(Resources.OAuthProviderOptionsNotFoundException, providerName));
        }
    }
}