using Shuttle.Core.Contract;

namespace Shuttle.OAuth
{
    public static class OAuthConfigurationExtensions
    {
        public static void ApplyInvariants(this IOAuthConfiguration configuration)
        {
            Guard.AgainstNull(configuration, nameof(configuration));
            
            Guard.AgainstNullOrEmptyString(configuration.Name, nameof(configuration.Name));
            Guard.AgainstNullOrEmptyString(configuration.ClientId, nameof(configuration.ClientId));
            Guard.AgainstNullOrEmptyString(configuration.ClientSecret, nameof(configuration.ClientSecret));
        }

        public static string GetTokenUrl(this IOAuthConfiguration configuration, string path)
        {
            Guard.AgainstNull(configuration, nameof(configuration));
            Guard.AgainstNull(configuration.TokenUrl, nameof(configuration.TokenUrl));

            return $"{configuration.TokenUrl}{(configuration.TokenUrl.EndsWith("/") ? string.Empty : "/")}{path}";
        }

        public static string GetDataUrl(this IOAuthConfiguration configuration, string path)
        {
            Guard.AgainstNull(configuration, nameof(configuration));
            Guard.AgainstNull(configuration.TokenUrl, nameof(configuration.TokenUrl));

            return $"{configuration.DataUrl}{(configuration.DataUrl.EndsWith("/") ? string.Empty : "/")}{path}";
        }
    }
}